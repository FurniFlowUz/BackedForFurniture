import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ArrowLeft, Upload, FileText } from 'lucide-react';

import { technicalSpecService } from '@/services/technicalSpecService';
import { orderService } from '@/services/orderService';
import { CreateTechnicalSpecificationDto } from '@/types';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const specSchema = z.object({
  orderId: z.string().min(1, 'Order is required'),
  specifications: z.string().min(20, 'Specifications must be at least 20 characters'),
});

type SpecFormData = z.infer<typeof specSchema>;

export function CreateTechnicalSpec() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [searchParams] = useSearchParams();
  const orderIdParam = searchParams.get('orderId');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [drawingFile, setDrawingFile] = useState<File | null>(null);
  const [uploadProgress, setUploadProgress] = useState(0);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
  } = useForm<SpecFormData>({
    resolver: zodResolver(specSchema),
    defaultValues: {
      orderId: orderIdParam || '',
    },
  });

  // Fetch orders that need technical specifications
  const { data: ordersData } = useQuery({
    queryKey: ['orders'],
    queryFn: () => orderService.getAll(1, 100),
  });

  const createSpecMutation = useMutation({
    mutationFn: (data: CreateTechnicalSpecificationDto) => technicalSpecService.create(data),
    onSuccess: async (result) => {
      // If there's a drawing file, upload it
      if (drawingFile) {
        try {
          await technicalSpecService.uploadDrawing(result.id, drawingFile, setUploadProgress);
          toast.success('Technical specification created with drawing!');
        } catch {
          toast.warning('Specification created but drawing upload failed');
        }
      } else {
        toast.success('Technical specification created successfully!');
      }

      queryClient.invalidateQueries({ queryKey: ['technical-specs'] });
      queryClient.invalidateQueries({ queryKey: ['orders'] });
      navigate('/orders');
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to create specification');
    },
  });

  const onSubmit = async (data: SpecFormData) => {
    setIsSubmitting(true);
    try {
      await createSpecMutation.mutateAsync(data);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];

      // Validate file type
      const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg', 'application/pdf'];
      if (!allowedTypes.includes(file.type)) {
        toast.error('Only JPG, PNG, and PDF files are allowed');
        return;
      }

      // Validate file size (max 10MB)
      if (file.size > 10 * 1024 * 1024) {
        toast.error('File size must be less than 10MB');
        return;
      }

      setDrawingFile(file);
    }
  };

  const orderOptions = ordersData?.items
    .filter((o) => o.status === 'InDesign' || o.status === 'Created')
    .map((order) => ({
      value: order.id,
      label: `${order.orderNumber} - ${order.furnitureType}`,
    })) || [];

  const selectedOrderId = watch('orderId');
  const selectedOrder = ordersData?.items.find((o) => o.id === selectedOrderId);

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
        <h1 className="text-3xl font-bold text-gray-900">Create Technical Specification</h1>
        <p className="mt-2 text-gray-600">Define technical details and upload design drawings</p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Card>
          <CardHeader>
            <CardTitle>Technical Specification</CardTitle>
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
                Only orders pending technical specifications are shown
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
                  <span className="text-gray-600">Description:</span>
                  <span className="font-medium col-span-1">{selectedOrder.description}</span>
                </div>
              </div>
            )}

            {/* Specifications */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Technical Specifications
                <span className="text-red-500 ml-1">*</span>
              </label>
              <textarea
                {...register('specifications')}
                rows={10}
                className="block w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 sm:text-sm font-mono"
                placeholder={`Enter detailed technical specifications:

- Materials: (e.g., Oak wood, MDF, Metal hinges)
- Dimensions: (e.g., Height: 200cm, Width: 100cm, Depth: 50cm)
- Construction: (e.g., Joinery methods, assembly details)
- Finish: (e.g., Paint type, varnish, laminate)
- Hardware: (e.g., Handles, hinges, slides)
- Special Requirements: (Any custom features or considerations)`}
              />
              {errors.specifications && (
                <p className="mt-1 text-sm text-red-600">{errors.specifications.message}</p>
              )}
            </div>

            {/* Drawing Upload */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Technical Drawing (Optional)
              </label>
              <div className="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-lg hover:border-primary-400 transition-colors">
                <div className="space-y-1 text-center">
                  {drawingFile ? (
                    <div className="flex items-center justify-center space-x-2">
                      <FileText className="h-8 w-8 text-primary-600" />
                      <span className="text-sm text-gray-600">{drawingFile.name}</span>
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={() => setDrawingFile(null)}
                      >
                        Remove
                      </Button>
                    </div>
                  ) : (
                    <>
                      <Upload className="mx-auto h-12 w-12 text-gray-400" />
                      <div className="flex text-sm text-gray-600">
                        <label className="relative cursor-pointer rounded-md font-medium text-primary-600 hover:text-primary-500">
                          <span>Upload a file</span>
                          <input
                            type="file"
                            className="sr-only"
                            accept="image/jpeg,image/png,image/jpg,application/pdf"
                            onChange={handleFileChange}
                          />
                        </label>
                        <p className="pl-1">or drag and drop</p>
                      </div>
                      <p className="text-xs text-gray-500">PNG, JPG, PDF up to 10MB</p>
                    </>
                  )}
                </div>
              </div>
            </div>

            {/* Upload Progress */}
            {uploadProgress > 0 && uploadProgress < 100 && (
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div
                  className="bg-primary-600 h-2 rounded-full transition-all duration-300"
                  style={{ width: `${uploadProgress}%` }}
                />
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
                Create Specification
              </Button>
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}
