// Enums
export enum UserRole {
  Director = 'Director',
  Seller = 'Seller',
  Constructor = 'Constructor',
  ProductionManager = 'ProductionManager',
  TeamLeader = 'TeamLeader',
  Worker = 'Worker',
  WarehouseManager = 'WarehouseManager',
}

export enum ContractStatus {
  Draft = 'Draft',
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
}

export enum OrderStatus {
  Created = 'Created',
  InDesign = 'InDesign',
  InProduction = 'InProduction',
  Completed = 'Completed',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled',
}

export enum ProductionTaskStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Blocked = 'Blocked',
}

export enum MaterialRequestStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  PartiallyIssued = 'PartiallyIssued',
  Issued = 'Issued',
  Rejected = 'Rejected',
}

export enum Priority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Critical = 'Critical',
}

// User
export interface User {
  id: string;
  username: string;
  fullName: string;
  email: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

// Contract
export interface Contract {
  id: string;
  contractNumber: string;
  clientName: string;
  clientPhone: string;
  clientAddress: string;
  totalAmount: number;
  advancePayment: number;
  startDate: string;
  endDate: string;
  status: ContractStatus;
  sellerId: string;
  seller?: User;
  orders?: Order[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateContractDto {
  clientName: string;
  clientPhone: string;
  clientAddress: string;
  totalAmount: number;
  advancePayment: number;
  startDate: string;
  endDate: string;
}

// Order
export interface Order {
  id: string;
  orderNumber: string;
  contractId: string;
  furnitureType: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  description: string;
  status: OrderStatus;
  contract?: Contract;
  technicalSpecification?: TechnicalSpecification;
  productionTasks?: ProductionTask[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateOrderDto {
  contractId: string;
  furnitureType: string;
  quantity: number;
  unitPrice: number;
  description: string;
}

// Technical Specification
export interface TechnicalSpecification {
  id: string;
  orderId: string;
  specifications: string;
  drawingUrl?: string;
  constructorId: string;
  constructor?: User;
  order?: Order;
  createdAt: string;
  updatedAt: string;
}

export interface CreateTechnicalSpecificationDto {
  orderId: string;
  specifications: string;
}

export interface UploadDrawingDto {
  file: File;
}

// Production Task
export interface ProductionTask {
  id: string;
  orderId: string;
  taskNumber: string;
  title: string;
  description: string;
  sequenceOrder: number;
  priority: Priority;
  status: ProductionTaskStatus;
  estimatedHours: number;
  actualHours?: number;
  assignedToId?: string;
  assignedTo?: User;
  startDate?: string;
  completedDate?: string;
  productionManagerId: string;
  productionManager?: User;
  order?: Order;
  dependencies?: TaskDependency[];
  dependents?: TaskDependency[];
  materialRequests?: MaterialRequest[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductionTaskDto {
  orderId: string;
  title: string;
  description: string;
  sequenceOrder: number;
  priority: Priority;
  estimatedHours: number;
  assignedToId?: string;
}

export interface AssignTaskDto {
  assignedToId: string;
}

export interface UpdateTaskStatusDto {
  status: ProductionTaskStatus;
  actualHours?: number;
}

// Task Dependency
export interface TaskDependency {
  id: string;
  taskId: string;
  dependsOnTaskId: string;
  task?: ProductionTask;
  dependsOnTask?: ProductionTask;
}

export interface CreateTaskDependencyDto {
  taskId: string;
  dependsOnTaskId: string;
}

// Material
export interface Material {
  id: string;
  name: string;
  category: string;
  unit: string;
  currentStock: number;
  minimumStock: number;
  unitPrice: number;
  supplierId?: string;
  supplier?: Supplier;
  createdAt: string;
  updatedAt: string;
}

export interface CreateMaterialDto {
  name: string;
  category: string;
  unit: string;
  currentStock: number;
  minimumStock: number;
  unitPrice: number;
  supplierId?: string;
}

// Supplier
export interface Supplier {
  id: string;
  name: string;
  contactPerson: string;
  phone: string;
  email?: string;
  address: string;
  materials?: Material[];
  createdAt: string;
  updatedAt: string;
}

export interface CreateSupplierDto {
  name: string;
  contactPerson: string;
  phone: string;
  email?: string;
  address: string;
}

// Material Request
export interface MaterialRequest {
  id: string;
  productionTaskId: string;
  materialId: string;
  requestedQuantity: number;
  issuedQuantity: number;
  status: MaterialRequestStatus;
  productionTask?: ProductionTask;
  material?: Material;
  createdAt: string;
  updatedAt: string;
}

export interface CreateMaterialRequestDto {
  productionTaskId: string;
  materialId: string;
  requestedQuantity: number;
}

export interface IssueMaterialDto {
  issuedQuantity: number;
}

// Notification
export interface Notification {
  id: string;
  userId: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  user?: User;
}

// Dashboard Stats
export interface DashboardStats {
  totalContracts: number;
  activeContracts: number;
  totalOrders: number;
  ordersInProduction: number;
  completedOrders: number;
  totalRevenue: number;
  pendingTasks: number;
  completedTasks: number;
}

// API Response types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
