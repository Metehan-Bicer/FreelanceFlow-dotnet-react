import { apiClient } from './apiClient';
import { 
  LoginRequest, 
  LoginResponse, 
  RegisterRequest,
  ChangePasswordRequest,
  ApiResponse,
  User
} from '../types';

export class AuthService {
  private readonly TOKEN_KEY = 'token';
  private readonly USER_KEY = 'user';

  async login(credentials: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    try {
      console.log('AuthService: Attempting login...'); // Debug log
      const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
      
      console.log('AuthService: Login response:', response); // Debug log
      
      if (response.success && response.data) {
        const token = response.data.token;
        const userData = {
          username: response.data.username,
          email: response.data.email,
          role: response.data.role
        };
        
        console.log('AuthService: Storing token and user data...'); // Debug log
        localStorage.setItem(this.TOKEN_KEY, token);
        localStorage.setItem(this.USER_KEY, JSON.stringify(userData));
        
        // Verify storage
        const storedToken = localStorage.getItem(this.TOKEN_KEY);
        const storedUser = localStorage.getItem(this.USER_KEY);
        console.log('AuthService: Stored token:', !!storedToken); // Debug log
        console.log('AuthService: Stored user:', storedUser); // Debug log
      }
      
      return response;
    } catch (error) {
      console.error('AuthService: Login error:', error); // Debug log
      return {
        success: false,
        error: 'Giriş yaparken bir hata oluştu'
      };
    }
  }

  async register(data: RegisterRequest): Promise<ApiResponse<string>> {
    return await apiClient.post<string>('/auth/register', data);
  }

  async changePassword(data: ChangePasswordRequest): Promise<ApiResponse<string>> {
    return await apiClient.post<string>('/auth/change-password', data);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getCurrentUser(): any | null {
    const userStr = localStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    const user = this.getCurrentUser();
    return !!(token && user);
  }
}

export const authService = new AuthService();