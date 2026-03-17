import apiClient from './client';

// --- Dashboard ---
export const dashboardApi = {
    getKpis: () => apiClient.get('/dashboard/kpis'),
    getActiveOrders: () => apiClient.get('/dashboard/active-orders'),
};

// --- Customers ---
export const customersApi = {
    create: (data: any) => apiClient.post('/customers', data),
    getAll: (params?: { searchTerm?: string; page?: number; pageSize?: number }) =>
        apiClient.get('/customers', { params }),
    getById: (id: string) => apiClient.get(`/customers/${id}`),
};

// --- Vehicles ---
export const vehiclesApi = {
    create: (data: any) => apiClient.post('/vehicles', data),
    getAll: (params?: { customerId?: string; searchTerm?: string; page?: number; pageSize?: number }) =>
        apiClient.get('/vehicles', { params }),
    getById: (id: string) => apiClient.get(`/vehicles/${id}`),
};

// --- Repair Orders ---
export const repairOrdersApi = {
    create: (data: any) => apiClient.post('/repairorders', data),
    getAll: (params?: { status?: string; searchTerm?: string; page?: number; pageSize?: number }) =>
        apiClient.get('/repairorders', { params }),
    getById: (id: string) => apiClient.get(`/repairorders/${id}`),
    addPhotos: (id: string, photoUrls: string[]) => apiClient.post(`/repairorders/${id}/photos`, photoUrls),
};

// --- Diagnostics ---
export const diagnosticsApi = {
    submit: (repairOrderId: string, data: any) =>
        apiClient.post(`/repairorders/${repairOrderId}/diagnostics`, data),
};

// --- Estimates ---
export const estimatesApi = {
    generate: (data: any) => apiClient.post('/estimates', data),
    approve: (id: string, data: any) => apiClient.post(`/estimates/${id}/approve`, data),
    refuse: (id: string) => apiClient.post(`/estimates/${id}/refuse`),
    getPdf: (id: string) => apiClient.get(`/estimates/${id}/pdf`, { responseType: 'blob' }),
    sendEmail: (id: string, toEmail: string) =>
        apiClient.post(`/estimates/${id}/send-email`, { toEmail }),
};

// --- Inventory ---
export const inventoryApi = {
    receiveStock: (data: any) => apiClient.post('/inventory/receive', data),
    issuePart: (data: any) => apiClient.post('/inventory/issue', data),
};

// --- Repairs ---
export const repairsApi = {
    start: (repairOrderId: string, data: any) =>
        apiClient.post(`/repairs/${repairOrderId}/start`, data),
    complete: (repairOrderId: string, data: any) =>
        apiClient.post(`/repairs/${repairOrderId}/complete`, data),
};

// --- Quality ---
export const qualityApi = {
    submitChecklist: (repairOrderId: string, data: any) =>
        apiClient.post(`/repairorders/${repairOrderId}/quality/submit`, data),
};

// --- Files ---
export const filesApi = {
    upload: (file: File, folder?: string) => {
        const formData = new FormData();
        formData.append('file', file);
        return apiClient.post('/files/upload', formData, {
            params: { folder },
            headers: { 'Content-Type': 'multipart/form-data' }
        });
    },
    uploadMultiple: (files: File[], folder?: string) => {
        const formData = new FormData();
        files.forEach(file => formData.append('files', file));
        return apiClient.post('/files/upload-multiple', formData, {
            params: { folder },
            headers: { 'Content-Type': 'multipart/form-data' }
        });
    },
};

// --- Billing ---
export const billingApi = {
    generateInvoice: (data: any) => apiClient.post('/billing/invoices', data),
    registerPayment: (invoiceId: string, data: any) =>
        apiClient.post(`/billing/invoices/${invoiceId}/pay`, data),
    getInvoicePdf: (id: string) =>
        apiClient.get(`/billing/invoices/${id}/pdf`, { responseType: 'blob' }),
};
