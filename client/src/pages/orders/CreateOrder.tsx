import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ArrowLeft } from 'lucide-react';

import { orderService } from '@/services/orderService';
import { contractService } from '@/services/contractService';
import { CreateOrderDto } from '@/types';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const orderSchema = z.object({
  contractId: z.string().min(1, 'Contract is required'),
  furnitureType: z.string().min(2, 'Furniture type must be at least 2 characters'),
  quantity: z.number().min(1, 'Quantity must be at least 1'),
  unitPrice: z.number().min(0, 'Unit price must be positive'),
  description: z.string().min(10, 'Description must be at least 10 characters'),
});

type OrderFormData = z.infer<typeof orderSchema>;

export function CreateOrder() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [searchParams] = useSearchParams();
  const contractIdParam = searchParams.get('contractId');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<OrderFormData>({
    resolver: zodResolver(orderSchema),
    defaultValues: {
      contractId: contractIdParam || '',
    },
  });

  // Fetch contracts for dropdown
  const { data: contractsData } = useQuery({
    queryKey: ['contracts'],
    queryFn: () => contractService.getMyContracts(1, 100),
  });

  const createOrderMutation = useMutation({
    mutationFn: (data: CreateOrderDto) => orderService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      toast.success('Order created successfully!');
      navigate('/orders');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to create order');
    },
  });

  const onSubmit = async (data: OrderFormData) => {
    setIsSubmitting(true);
    try {
      await createOrderMutation.mutateAsync(data);
    } finally {
      setIsSubmitting(false);
    }
  };

  const quantity = watch('quantity', 1);
  const unitPrice = watch('unitPrice', 0);
  const totalPrice = quantity * unitPrice;

  const contractOptions = contractsData?.items
    .filter((c) => c.status === 'Active')
    .map((contract) => ({
      value: contract.id,
      label: `${contract.contractNumber} - ${contract.clientName}`,
    })) || [];

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate('/orders')}
          className="mb-4"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Orders
        </Button>
        <h1 className="text-3xl font-bold text-gray-900">Create New Order</h1>
        <p className="mt-2 text-gray-600">Add a furniture order to an existing contract</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Card>
          <CardHeader>
            <CardTitle>Order Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Contract Selection */}
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-4">Contract</h3>
              <Select
                label="Select Contract"
                {...register('contractId')}
                error={errors.contractId?.message}
                options={contractOptions}
                required
              />
              <p className="mt-2 text-sm text-gray-500">
                Only active contracts are shown
              </p>
            </div>

            {/* Furniture Information */}
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-4">Furniture Details</h3>
              <div className="space-y-4">
                <Input
                  label="Furniture Type"
                  {...register('furnitureType')}
                  error={errors.furnitureType?.message}
                  placeholder="e.g., Kitchen Cabinet, Bedroom Set, Office Desk"
                  required
                />
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <Input
                    label="Quantity"
                    type="number"
                    {...register('quantity', { valueAsNumber: true })}
                    error={errors.quantity?.message}
                    placeholder="1"
                    min="1"
                    required
                  />
                  <Input
                    label="Unit Price (UZS)"
                    type="number"
                    step="0.01"
                    {...register('unitPrice', { valueAsNumber: true })}
                    error={errors.unitPrice?.message}
                    placeholder="1000000"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Description
                    <span className="text-red-500 ml-1">*</span>
                  </label>
                  <textarea
                    {...register('description')}
                    rows={4}
                    className="block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
                    placeholder="Detailed description of the furniture, including materials, dimensions, special requirements, etc."
                  />
                  {errors.description && (
                    <p className="mt-1 text-sm text-red-600">{errors.description.message}</p>
                  )}
                </div>
              </div>
            </div>

            {/* Price Summary */}
            {quantity > 0 && unitPrice > 0 && (
              <div className="p-4 bg-primary-50 rounded-lg border border-primary-200">
                <div className="flex justify-between items-center">
                  <div>
                    <p className="text-sm text-gray-600">
                      {quantity} × {unitPrice.toLocaleString()} UZS
                    </p>
                    <p className="text-xs text-gray-500 mt-1">Total Price</p>
                  </div>
                  <div className="text-right">
                    <p className="text-2xl font-bold text-primary-700">
                      {totalPrice.toLocaleString()}
                    </p>
                    <p className="text-xs text-gray-600">UZS</p>
                  </div>
                </div>
              </div>
            )}

            {/* Actions */}
            <div className="flex justify-end space-x-4 pt-4 border-t border-gray-200">
              <Button
                type="button"
                variant="secondary"
                onClick={() => navigate('/orders')}
                disabled={isSubmitting}
              >
                Cancel
              </Button>
              <Button type="submit" isLoading={isSubmitting} disabled={isSubmitting}>
                Create Order
              </Button>
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
