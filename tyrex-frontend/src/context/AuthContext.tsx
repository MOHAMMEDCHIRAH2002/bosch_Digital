import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { authApi, type LoginRequest } from '../api/auth';

interface User {
    userId: string;
    email: string;
    role: string;
}

interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (data: LoginRequest) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const storedUser = localStorage.getItem('tyrex_user');
        const token = localStorage.getItem('tyrex_access_token');
        if (storedUser && token) {
            setUser(JSON.parse(storedUser));
        }
        setIsLoading(false);
    }, []);

    const login = async (data: LoginRequest) => {
        const response = await authApi.login(data);
        const { accessToken, refreshToken, userId, email, role } = response.data;
        localStorage.setItem('tyrex_access_token', accessToken);
        localStorage.setItem('tyrex_refresh_token', refreshToken);
        const userData: User = { userId, email, role };
        localStorage.setItem('tyrex_user', JSON.stringify(userData));
        setUser(userData);
    };

    const logout = () => {
        localStorage.removeItem('tyrex_access_token');
        localStorage.removeItem('tyrex_refresh_token');
        localStorage.removeItem('tyrex_user');
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within an AuthProvider');
    return context;
}
