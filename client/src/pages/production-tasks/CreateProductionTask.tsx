import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ArrowLeft } from 'lucide-react';

import { productionTaskService } from '@/services/productionTaskService';
import { orderService } from '@/services/orderService';
import { CreateProductionTaskDto, Priority } from '@/types';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const taskSchema = z.object({
  orderId: z.string().min(1, 'Order is required'),
  title: z.string().min(3, 'Title must be at least 3 characters'),
  description: z.string().min(10, 'Description must be at least 10 characters'),
  sequenceOrder: z.number().min(1, 'Sequence order must be at least 1'),
  priority: z.nativeEnum(Priority),
  estimatedHours: z.number().min(0.5, 'Estimated hours must be at least 0.5'),
  assignedToId: z.string().optional(),
});

type TaskFormData = z.infer<typeof taskSchema>;

export function CreateProductionTask() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [searchParams] = useSearchParams();
  const orderIdParam = searchParams.get('orderId');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<TaskFormData>({
    resolver: zodResolver(taskSchema),
    defaultValues: {
      orderId: orderIdParam || '',
      priority: Priority.Medium,
      sequenceOrder: 1,
    },
  });

  // Fetch orders for selection
  const { data: ordersData } = useQuery({
    queryKey: ['orders'],
    queryFn: () => orderService.getAll(1, 100),
  });

  const createTaskMutation = useMutation({
    mutationFn: (data: CreateProductionTaskDto) => productionTaskService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['production-tasks'] });
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      toast.success('Production task created successfully!');
      navigate('/production-tasks');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to create task');
    },
  });

  const onSubmit = async (data: TaskFormData) => {
    setIsSubmitting(true);
    try {
      const payload: CreateProductionTaskDto = {
        ...data,
        assignedToId: data.assignedToId || undefined,
      };
      await createTaskMutation.mutateAsync(payload);
    } finally {
      setIsSubmitting(false);
    }
  };

  const orderOptions = ordersData?.items
    .filter((o) => o.status === 'InProduction' || o.status === 'InDesign')
    .map((order) => ({
      value: order.id,
      label: `${order.orderNumber} - ${order.furnitureType}`,
    })) || [];

  const priorityOptions = [
    { value: Priority.Low, label: 'Low' },
    { value: Priority.Medium, label: 'Medium' },
    { value: Priority.High, label: 'High' },
    { value: Priority.Critical, label: 'Critical' },
  ];

  const selectedOrderId = watch('orderId');
  const selectedOrder = ordersData?.items.find((o) => o.id === selectedOrderId);

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate('/production-tasks')}
          className="mb-4"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Production Tasks
        </Button>
        <h1 className="text-3xl font-bold text-gray-900">Create Production Task</h1>
        <p className="mt-2 text-gray-600">Create a new task for furniture production</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Card>
          <CardHeader>
            <CardTitle>Task Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Order Selection */}
            <div>
              <Select
                label="Select Order"
                {...register('orderId')}
                error={errors.orderId?.message}
                options={orderOptions}
                required
              />
              <p className="mt-2 text-sm text-gray-500">
                Only orders in production or design phase are shown
              </p>
            </div>

            {/* Order Details */}
            {selectedOrder && (
              <div className="p-4 bg-gray-50 rounded-lg border border-gray-200">
                <h4 className="font-semibold text-gray-900 mb-2">Order Details</h4>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <span className="text-gray-600">Furniture:</span>
                  <span className="font-medium">{selectedOrder.furnitureType}</span>
                  <span className="text-gray-600">Quantity:</span>
                  <span className="font-medium">{selectedOrder.quantity}</span>
                  <span className="text-gray-600">Status:</span>
                  <span className="font-medium">{selectedOrder.status}</span>
                </div>
              </div>
            )}

            {/* Task Details */}
            <div>
              <h3 className="text-sm font-semibold text-gray-900 mb-4">Task Details</h3>
              <div className="space-y-4">
                <Input
                  label="Task Title"
                  {...register('title')}
                  error={errors.title?.message}
                  placeholder="e.g., Cut wooden panels, Assemble frame, Apply finish"
                  required
                />

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Description
                    <span className="text-red-500 ml-1">*</span>
                  </label>
                  <textarea
                    {...register('description')}
                    rows={4}
                    className="block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
                    placeholder="Detailed description of the task, including specific requirements and quality standards..."
                  />
                  {errors.description && (
                    <p className="mt-1 text-sm text-red-600">{errors.description.message}</p>
                  )}
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <Input
                    label="Sequence Order"
                    type="number"
                    {...register('sequenceOrder', { valueAsNumber: true })}
                    error={errors.sequenceOrder?.message}
                    placeholder="1"
                    min="1"
                    helperText="Order in which this task should be executed"
                    required
                  />

                  <Select
                    label="Priority"
                    {...register('priority')}
                    error={errors.priority?.message}
                    options={priorityOptions}
                    required
                  />

                  <Input
                    label="Estimated Hours"
                    type="number"
                    step="0.5"
                    {...register('estimatedHours', { valueAsNumber: true })}
                    error={errors.estimatedHours?.message}
                    placeholder="8"
                    min="0.5"
                    required
                  />
                </div>
              </div>
            </div>

            {/* Info Box */}
            <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
              <p className="text-sm text-blue-800">
                <strong>Note:</strong> You can assign this task to a team member later from the task details page.
                Task dependencies can also be configured after creation.
              </p>
            </div>

            {/* Actions */}
            <div className="flex justify-end space-x-4 pt-4 border-t border-gray-200">
              <Button
                type="button"
                variant="secondary"
                onClick={() => navigate('/production-tasks')}
                disabled={isSubmitting}
              >
                Cancel
              </Button>
              <Button type="submit" isLoading={isSubmitting} disabled={isSubmitting}>
                Create Task
              </Button>
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
