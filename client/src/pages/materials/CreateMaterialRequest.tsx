import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ArrowLeft } from 'lucide-react';

import { materialRequestService, materialService } from '@/services/materialService';
import { productionTaskService } from '@/services/productionTaskService';
import { CreateMaterialRequestDto } from '@/types';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const requestSchema = z.object({
  productionTaskId: z.string().min(1, 'Production task is required'),
  materialId: z.string().min(1, 'Material is required'),
  requestedQuantity: z.number().min(0.01, 'Requested quantity must be greater than 0'),
});

type RequestFormData = z.infer<typeof requestSchema>;

export function CreateMaterialRequest() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [searchParams] = useSearchParams();
  const taskIdParam = searchParams.get('taskId');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<RequestFormData>({
    resolver: zodResolver(requestSchema),
    defaultValues: {
      productionTaskId: taskIdParam || '',
    },
  });

  // Fetch user's tasks
  const { data: tasks } = useQuery({
    queryKey: ['my-tasks'],
    queryFn: () => productionTaskService.getMyTasks(),
  });

  // Fetch materials
  const { data: materialsData } = useQuery({
    queryKey: ['materials'],
    queryFn: () => materialService.getAll(1, 500),
  });

  const createRequestMutation = useMutation({
    mutationFn: (data: CreateMaterialRequestDto) => materialRequestService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['material-requests'] });
      toast.success('Material request submitted successfully!');
      navigate('/production-tasks/my');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to create request');
    },
  });

  const onSubmit = async (data: RequestFormData) => {
    setIsSubmitting(true);
    try {
      await createRequestMutation.mutateAsync(data);
    } finally {
      setIsSubmitting(false);
    }
  };

  const taskOptions = tasks?.map((task) => ({
    value: task.id,
    label: `${task.taskNumber} - ${task.title}`,
  })) || [];

  const materialOptions = materialsData?.items.map((material) => ({
    value: material.id,
    label: `${material.name} (${material.category})`,
  })) || [];

  const selectedTaskId = watch('productionTaskId');
  const selectedTask = tasks?.find((t) => t.id === selectedTaskId);

  const selectedMaterialId = watch('materialId');
  const selectedMaterial = materialsData?.items.find((m) => m.id === selectedMaterialId);

  const requestedQty = watch('requestedQuantity', 0);

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-6">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => navigate('/production-tasks/my')}
          className="mb-4"
        >
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to My Tasks
        </Button>
        <h1 className="text-3xl font-bold text-gray-900">Request Material</h1>
        <p className="mt-2 text-gray-600">Submit a material request for your production task</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Card>
          <CardHeader>
            <CardTitle>Material Request</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Task Selection */}
            <div>
              <Select
                label="Production Task"
                {...register('productionTaskId')}
                error={errors.productionTaskId?.message}
                options={taskOptions}
                required
              />
              <p className="mt-2 text-sm text-gray-500">
                Select the task you need materials for
              </p>
            </div>

            {/* Task Details */}
            {selectedTask && (
              <div className="p-4 bg-gray-50 rounded-lg border border-gray-200">
                <h4 className="font-semibold text-gray-900 mb-2">Task Details</h4>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <span className="text-gray-600">Task:</span>
                  <span className="font-medium">{selectedTask.title}</span>
                  <span className="text-gray-600">Status:</span>
                  <span className="font-medium">{selectedTask.status}</span>
                  <span className="text-gray-600">Description:</span>
                  <span className="font-medium col-span-1">{selectedTask.description}</span>
                </div>
              </div>
            )}

            {/* Material Selection */}
            <div>
              <Select
                label="Material"
                {...register('materialId')}
                error={errors.materialId?.message}
                options={materialOptions}
                required
              />
            </div>

            {/* Material Details */}
            {selectedMaterial && (
              <div className="p-4 bg-blue-50 rounded-lg border border-blue-200">
                <h4 className="font-semibold text-gray-900 mb-2">Material Information</h4>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <span className="text-gray-600">Name:</span>
                  <span className="font-medium">{selectedMaterial.name}</span>
                  <span className="text-gray-600">Category:</span>
                  <span className="font-medium">{selectedMaterial.category}</span>
                  <span className="text-gray-600">Unit:</span>
                  <span className="font-medium">{selectedMaterial.unit}</span>
                  <span className="text-gray-600">Available Stock:</span>
                  <span className={`font-semibold ${
                    selectedMaterial.currentStock > selectedMaterial.minimumStock
                      ? 'text-green-600'
                      : 'text-red-600'
                  }`}>
                    {selectedMaterial.currentStock} {selectedMaterial.unit}
                  </span>
                </div>
              </div>
            )}

            {/* Quantity Input */}
            <div>
              <Input
                label={`Requested Quantity${selectedMaterial ? ` (${selectedMaterial.unit})` : ''}`}
                type="number"
                step="0.01"
                {...register('requestedQuantity', { valueAsNumber: true })}
                error={errors.requestedQuantity?.message}
                placeholder="0"
                min="0.01"
                required
              />
            </div>

            {/* Request Summary */}
            {requestedQty > 0 && selectedMaterial && (
              <div className="p-4 bg-primary-50 border border-primary-200 rounded-lg">
                <h4 className="font-semibold text-gray-900 mb-3">Request Summary</h4>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Material:</span>
                    <span className="font-medium">{selectedMaterial.name}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Requested:</span>
                    <span className="font-semibold text-primary-700">
                      {requestedQty} {selectedMaterial.unit}
                    </span>
                  </div>
                  {requestedQty > selectedMaterial.currentStock && (
                    <div className="mt-2 p-2 bg-yellow-50 border border-yellow-200 rounded text-xs text-yellow-800">
                      <strong>Warning:</strong> Requested quantity exceeds available stock.
                      This request will need warehouse manager approval and may require restocking.
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Info Box */}
            <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
              <p className="text-sm text-blue-800">
                <strong>Note:</strong> Your request will be sent to the warehouse manager for approval.
                You'll be notified once the materials are ready for pickup.
              </p>
            </div>

            {/* Actions */}
            <div className="flex justify-end space-x-4 pt-4 border-t border-gray-200">
              <Button
                type="button"
                variant="secondary"
                onClick={() => navigate('/production-tasks/my')}
                disabled={isSubmitting}
              >
                Cancel
              </Button>
              <Button type="submit" isLoading={isSubmitting} disabled={isSubmitting}>
                Submit Request
              </Button>
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
