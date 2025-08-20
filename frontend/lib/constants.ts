export class Constants {
  static readonly API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:7080';
  
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
      DELETE: (id: string) => `/api/product/${id}`,
      // No separate upload; image file is sent with product create/update
    }
  };
  
  static getApiUrl(endpoint: string): string {
    return `${this.API_BASE_URL}${endpoint}`;
  }

  static getImageUrl(guidOrName?: string | null): string {
    if (!guidOrName) return 'https://via.placeholder.com/800x600';
    // API serves images at '/image/{guid}'
    return `${this.API_BASE_URL}/image/${guidOrName}`;
  }
} 