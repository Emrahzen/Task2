export class Constants {
  static readonly API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5077';
  
  static readonly API_ENDPOINTS = {
    AUTH: {
      LOGIN: '/api/auth/login',
      REGISTER: '/api/auth/register'
    },
    PRODUCT: {
      GET_ALL: '/api/product',
      GET_BY_ID: (id: string) => `/api/product/${id}`,
      GET_BY_CATEGORY: '/api/product/category',
      SEARCH: '/api/product/search',
      CREATE: '/api/product',
      UPDATE: (id: string) => `/api/product/${id}`,
      DELETE: (id: string) => `/api/product/${id}`
    }
  };
  
  static getApiUrl(endpoint: string): string {
    return `${this.API_BASE_URL}${endpoint}`;
  }
} 