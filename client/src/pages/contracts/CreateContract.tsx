import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ArrowLeft } from 'lucide-react';

import { contractService } from '@/services/contractService';
import { CreateContractDto } from '@/types';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const contractSchema = z.object({
  clientName: z.string().min(2, 'Client name must be at least 2 characters'),
  clientPhone: z.string().min(9, 'Valid phone number required'),
  clientAddress: z.string().min(5, 'Address must be at least 5 characters'),
  totalAmount: z.number().min(0, 'Total amount must be positive'),
  advancePayment: z.number().min(0, 'Advance payment must be positive'),
  startDate: z.string().min(1, 'Start date is required'),
  endDate: z.string().min(1, 'End date is required'),
}).refine((data) => {
  return new Date(data.endDate) > new Date(data.startDate);
}, {
  message: 'End date must be after start date',
  path: ['endDate'],
}).refine((data) => {
  return data.advancePayment <= data.totalAmount;
}, {
  message: 'Advance payment cannot exceed total amount',
  path: ['advancePayment'],
});

type ContractFormData = z.infer<typeof contractSchema>;

export function CreateContract() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<ContractFormData>({
    resolver: zodResolver(contractSchema),
  });

  const createContractMutation = useMutation({
    mutationFn: (data: CreateContractDto) => contractService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['contracts'] });
      toast.success('Contract created successfully!');
      navigate('/contracts');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to create contract');
    },
  });

  const onSubmit = async (data: ContractFormData) => {
    setIsSubmitting(true);
    try {
      await createContractMutation.mutateAsync(data);
    } finally {
      setIsSubmitting(false);
    }
  };

  const totalAmount = watch('totalAmount', 0);
  const advancePayment = watch('advancePayment', 0);
  const remainingAmount = totalAmount - advancePayment;

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate('/contracts')}
          className="mb-4"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Contracts
        </Button>
        <h1 className="text-3xl font-bold text-gray-900">Create New Contract</h1>
        <p className="mt-2 text-gray-600">Enter contract details for a new client</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Card>
          <CardHeader>
            <CardTitle>Contract Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Client Information */}
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-4">Client Information</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                  label="Client Name"
                  {...register('clientName')}
                  error={errors.clientName?.message}
                  placeholder="John Doe"
                  required
                />
                <Input
                  label="Client Phone"
                  {...register('clientPhone')}
                  error={errors.clientPhone?.message}
                  placeholder="+998901234567"
                  required
                />
              </div>
              <div className="mt-4">
                <Input
                  label="Client Address"
                  {...register('clientAddress')}
                  error={errors.clientAddress?.message}
                  placeholder="123 Main St, Tashkent"
                  required
                />
              </div>
            </div>

            {/* Financial Information */}
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-4">Financial Information</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                  label="Total Amount (UZS)"
                  type="number"
                  step="0.01"
                  {...register('totalAmount', { valueAsNumber: true })}
                  error={errors.totalAmount?.message}
                  placeholder="10000000"
                  required
                />
                <Input
                  label="Advance Payment (UZS)"
                  type="number"
                  step="0.01"
                  {...register('advancePayment', { valueAsNumber: true })}
                  error={errors.advancePayment?.message}
                  placeholder="5000000"
                  required
                />
              </div>
              {totalAmount > 0 && (
                <div className="mt-4 p-4 bg-gray-50 rounded-lg">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Total Amount:</span>
                    <span className="font-semibold">{totalAmount.toLocaleString()} UZS</span>
                  </div>
                  <div className="flex justify-between text-sm mt-2">
                    <span className="text-gray-600">Advance Payment:</span>
                    <span className="font-semibold">{advancePayment.toLocaleString()} UZS</span>
                  </div>
                  <div className="flex justify-between text-sm mt-2 pt-2 border-t border-gray-300">
                    <span className="text-gray-900 font-medium">Remaining:</span>
                    <span className="font-bold text-primary-600">
                      {remainingAmount.toLocaleString()} UZS
                    </span>
                  </div>
                </div>
              )}
            </div>

            {/* Date Information */}
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-4">Timeline</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <Input
                  label="Start Date"
                  type="date"
                  {...register('startDate')}
                  error={errors.startDate?.message}
                  required
                />
                <Input
                  label="End Date"
                  type="date"
                  {...register('endDate')}
                  error={errors.endDate?.message}
                  required
                />
              </div>
            </div>

            {/* Actions */}
            <div className="flex justify-end space-x-4 pt-4 border-t border-gray-200">
              <Button
                type="button"
                variant="secondary"
                onClick={() => navigate('/contracts')}
                disabled={isSubmitting}
              >
                Cancel
              </Button>
              <Button type="submit" isLoading={isSubmitting} disabled={isSubmitting}>
                Create Contract
              </Button>
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
