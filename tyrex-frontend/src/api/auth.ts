import apiClient from './client';

export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    accessToken: string;
    refreshToken: string;
    userId: string;
    email: string;
    role: string;
}

export const authApi = {
    login: (data: LoginRequest) => apiClient.post<LoginResponse>('/auth/login', data),
    refresh: (refreshToken: string) => apiClient.post<LoginResponse>('/auth/refresh', { refreshToken }),
};
