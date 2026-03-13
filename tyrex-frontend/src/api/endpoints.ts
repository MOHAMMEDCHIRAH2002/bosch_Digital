import apiClient from './client';

// --- Dashboard ---
export const dashboardApi = {
    getKpis: () => apiClient.get('/dashboard/kpis'),
    getActiveOrders: () => apiClient.get('/dashboard/active-orders'),
};

// --- Customers ---
export const customersApi = {
    create: (data: any) => apiClient.post('/customers', data),
    getAll: () => apiClient.get('/customers'),
};

// --- Vehicles ---
export const vehiclesApi = {
    create: (data: any) => apiClient.post('/vehicles', data),
};

// --- Repair Orders ---
export const repairOrdersApi = {
    create: (data: any) => apiClient.post('/repair-orders', data),
};

// --- Diagnostics ---
export const diagnosticsApi = {
    submit: (repairOrderId: string, data: any) =>
        apiClient.post(`/repair-orders/${repairOrderId}/diagnostics`, data),
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
        apiClient.post(`/repair-orders/${repairOrderId}/quality/submit`, data),
};

// --- Billing ---
export const billingApi = {
    generateInvoice: (data: any) => apiClient.post('/billing/invoices', data),
    registerPayment: (invoiceId: string, data: any) =>
        apiClient.post(`/billing/invoices/${invoiceId}/pay`, data),
    getInvoicePdf: (id: string) =>
        apiClient.get(`/billing/invoices/${id}/pdf`, { responseType: 'blob' }),
};
