import React, { useState, useEffect, useContext, createContext, ReactNode } from 'react';
import { authService } from '../services/authService';
import { LoginRequest, LoginResponse, ApiResponse } from '../types';

interface AuthContextType {
  user: any | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (credentials: LoginRequest) => Promise<ApiResponse<LoginResponse>>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [user, setUser] = useState<any | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initializeAuth = () => {
      try {
        const token = authService.getToken();
        const savedUser = authService.getCurrentUser();
        
        if (token && savedUser) {
          setUser(savedUser);
        }
      } catch (error) {
        console.error('Error initializing auth:', error);
        authService.logout();
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const login = async (credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> => {
    setLoading(true);
    try {
      const response = await authService.login(credentials);
      if (response.success && response.data) {
        const userData = {
          username: response.data.username,
          email: response.data.email,
          role: response.data.role
        };
        setUser(userData);
        console.log('User set in context:', userData); // Debug log
      }
      return response;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    loading,
    login,
    logout,
  };

  console.log('AuthContext value:', { user, isAuthenticated: !!user, loading }); // Debug log

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};