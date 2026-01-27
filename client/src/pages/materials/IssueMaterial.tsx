import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ArrowLeft, Package, AlertCircle } from 'lucide-react';

import { materialRequestService } from '@/services/materialService';
import { IssueMaterialDto } from '@/types';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const issueMaterialSchema = z.object({
  issuedQuantity: z.number().min(0.01, 'Issued quantity must be greater than 0'),
});

type IssueMaterialFormData = z.infer<typeof issueMaterialSchema>;

export function IssueMaterial() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { data: request, isLoading } = useQuery({
    queryKey: ['material-request', id],
    queryFn: () => materialRequestService.getById(id!),
    enabled: !!id,
  });

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<IssueMaterialFormData>({
    resolver: zodResolver(issueMaterialSchema),
  });

  const issueMaterialMutation = useMutation({
    mutationFn: (data: IssueMaterialDto) => materialRequestService.issue(id!, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['material-requests'] });
      queryClient.invalidateQueries({ queryKey: ['materials'] });
      toast.success('Material issued successfully!');
      navigate('/materials/requests');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to issue material');
    },
  });

  const onSubmit = async (data: IssueMaterialFormData) => {
    setIsSubmitting(true);
    try {
      await issueMaterialMutation.mutateAsync(data);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return <div className="flex justify-center items-center h-64">Loading...</div>;
  }

  if (!request) {
    return (
      <div className="max-w-2xl mx-auto mt-8">
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12">
            <AlertCircle className="h-12 w-12 text-red-500 mb-4" />
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Request Not Found</h3>
            <p className="text-gray-600 mb-4">The material request you're looking for doesn't exist.</p>
            <Button onClick={() => navigate('/materials/requests')}>
              Back to Requests
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  const requestedQty = request.requestedQuantity;
  const alreadyIssued = request.issuedQuantity;
  const remaining = requestedQty - alreadyIssued;
  const issuedQty = watch('issuedQuantity', 0);
  const availableStock = request.material?.currentStock || 0;
  const canIssue = issuedQty > 0 && issuedQty <= remaining && issuedQty <= availableStock;

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate('/materials/requests')}
          className="mb-4"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Material Requests
        </Button>
        <h1 className="text-3xl font-bold text-gray-900">Issue Material</h1>
        <p className="mt-2 text-gray-600">Issue materials for production task</p>
      </div>

      <div className="space-y-6">
        {/* Request Details */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <Package className="h-5 w-5 mr-2" />
              Request Details
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="text-gray-600">Material:</span>
                <p className="font-semibold text-gray-900">{request.material?.name}</p>
              </div>
              <div>
                <span className="text-gray-600">Category:</span>
                <p className="font-medium">{request.material?.category}</p>
              </div>
              <div>
                <span className="text-gray-600">Unit:</span>
                <p className="font-medium">{request.material?.unit}</p>
              </div>
              <div>
                <span className="text-gray-600">Status:</span>
                <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                  request.status === 'Approved' ? 'bg-green-100 text-green-800' :
                  request.status === 'PartiallyIssued' ? 'bg-yellow-100 text-yellow-800' :
                  'bg-gray-100 text-gray-800'
                }`}>
                  {request.status}
                </span>
              </div>
            </div>

            <div className="border-t pt-4">
              <div className="grid grid-cols-3 gap-4 text-center">
                <div className="p-3 bg-blue-50 rounded-lg">
                  <p className="text-xs text-gray-600 mb-1">Requested</p>
                  <p className="text-lg font-bold text-blue-700">
                    {requestedQty} {request.material?.unit}
                  </p>
                </div>
                <div className="p-3 bg-green-50 rounded-lg">
                  <p className="text-xs text-gray-600 mb-1">Already Issued</p>
                  <p className="text-lg font-bold text-green-700">
                    {alreadyIssued} {request.material?.unit}
                  </p>
                </div>
                <div className="p-3 bg-orange-50 rounded-lg">
                  <p className="text-xs text-gray-600 mb-1">Remaining</p>
                  <p className="text-lg font-bold text-orange-700">
                    {remaining} {request.material?.unit}
                  </p>
                </div>
              </div>
            </div>

            <div className="p-3 bg-gray-50 rounded-lg">
              <div className="flex justify-between items-center">
                <span className="text-sm text-gray-600">Available Stock:</span>
                <span className={`text-sm font-semibold ${
                  availableStock >= remaining ? 'text-green-600' : 'text-red-600'
                }`}>
                  {availableStock} {request.material?.unit}
                </span>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Issue Form */}
        <form onSubmit={handleSubmit(onSubmit)}>
          <Card>
            <CardHeader>
              <CardTitle>Issue Quantity</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <Input
                label={`Quantity to Issue (${request.material?.unit})`}
                type="number"
                step="0.01"
                {...register('issuedQuantity', { valueAsNumber: true })}
                error={errors.issuedQuantity?.message}
                placeholder="0"
                max={Math.min(remaining, availableStock)}
                helperText={`Maximum: ${Math.min(remaining, availableStock)} ${request.material?.unit}`}
                required
              />

              {issuedQty > 0 && (
                <div className="p-4 bg-primary-50 border border-primary-200 rounded-lg">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm font-medium text-gray-700">You are issuing:</span>
                    <span className="text-lg font-bold text-primary-700">
                      {issuedQty} {request.material?.unit}
                    </span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-sm text-gray-600">Remaining after issue:</span>
                    <span className="text-sm font-semibold text-gray-900">
                      {(remaining - issuedQty).toFixed(2)} {request.material?.unit}
                    </span>
                  </div>
                </div>
              )}

              {issuedQty > remaining && (
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg flex items-start">
                  <AlertCircle className="h-5 w-5 text-red-600 mr-2 mt-0.5" />
                  <p className="text-sm text-red-800">
                    Issued quantity cannot exceed remaining quantity ({remaining} {request.material?.unit})
                  </p>
                </div>
              )}

              {issuedQty > availableStock && (
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg flex items-start">
                  <AlertCircle className="h-5 w-5 text-red-600 mr-2 mt-0.5" />
                  <p className="text-sm text-red-800">
                    Insufficient stock! Available: {availableStock} {request.material?.unit}
                  </p>
                </div>
              )}

              {/* Actions */}
              <div className="flex justify-end space-x-4 pt-4 border-t border-gray-200">
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => navigate('/materials/requests')}
                  disabled={isSubmitting}
                >
                  Cancel
                </Button>
                <Button
                  type="submit"
                  isLoading={isSubmitting}
                  disabled={isSubmitting || !canIssue}
                >
                  Issue Material
                </Button>
              </div>
            </CardContent>
          </Card>
        </form>
      </div>
    </div>
  );
}
