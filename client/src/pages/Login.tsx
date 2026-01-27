import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { Wrench, LogIn } from 'lucide-react';

import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/Card';

const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setIsSubmitting(true);
    try {
      await login(data);
      toast.success('Login successful!');
      navigate('/dashboard');
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Invalid email or password');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary-50 to-primary-100">
      <div className="w-full max-w-md px-6">
        {/* Logo and Title */}
        <div className="text-center mb-8">
          <div className="flex justify-center mb-4">
            <div className="bg-primary-600 p-4 rounded-2xl shadow-lg">
              <Wrench className="h-12 w-12 text-white" />
            </div>
          </div>
          <h1 className="text-4xl font-bold text-gray-900 mb-2">FurniFlowUz</h1>
          <p className="text-gray-600">Furniture Manufacturing ERP System</p>
        </div>

        {/* Login Card */}
        <Card>
          <CardHeader>
            <CardTitle className="text-center text-2xl">Sign In</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
              <Input
                label="Email"
                type="email"
                {...register('email')}
                error={errors.email?.message}
                placeholder="seller@furniflow.uz"
                autoComplete="email"
                autoFocus
                required
              />

              <Input
                label="Password"
                type="password"
                {...register('password')}
                error={errors.password?.message}
                placeholder="Enter your password"
                autoComplete="current-password"
                required
              />

              <Button
                type="submit"
                className="w-full"
                size="lg"
                isLoading={isSubmitting}
                disabled={isSubmitting}
              >
                <LogIn className="h-5 w-5 mr-2" />
                Sign In
              </Button>
            </form>

            {/* Demo Credentials */}
            <div className="mt-6 pt-6 border-t border-gray-200">
              <p className="text-sm text-gray-600 text-center mb-3">Demo Credentials:</p>
              <div className="grid grid-cols-2 gap-2 text-xs">
                <div className="p-2 bg-gray-50 rounded">
                  <strong>Director:</strong><br />director@furniflow.uz<br />password123
                </div>
                <div className="p-2 bg-gray-50 rounded">
                  <strong>Seller:</strong><br />seller@furniflow.uz<br />password123
                </div>
                <div className="p-2 bg-gray-50 rounded">
                  <strong>Constructor:</strong><br />constructor@furniflow.uz<br />password123
                </div>
                <div className="p-2 bg-gray-50 rounded">
                  <strong>Prod. Manager:</strong><br />pm@furniflow.uz<br />password123
                </div>
                <div className="p-2 bg-gray-50 rounded">
                  <strong>Team Leader:</strong><br />teamlead@furniflow.uz<br />password123
                </div>
                <div className="p-2 bg-gray-50 rounded">
                  <strong>Worker:</strong><br />worker@furniflow.uz<br />password123
                </div>
                <div className="p-2 bg-gray-50 rounded col-span-2">
                  <strong>Warehouse Manager:</strong><br />warehouse@furniflow.uz / password123
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Footer */}
        <p className="text-center text-sm text-gray-600 mt-6">
          © 2025 FurniFlowUz. All rights reserved.
        </p>
      </div>
    </div>
  );
}
