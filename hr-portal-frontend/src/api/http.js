import { API_BASE_URL, TOKEN_STORAGE_KEY } from '../config.js';

export async function http(path, options = {}) {
  try {
    const token = localStorage.getItem(TOKEN_STORAGE_KEY);
    const headers = new Headers(options.headers || {});
    const isForm = options.body instanceof FormData;

    if (!isForm && !headers.has('Content-Type')) {
      headers.set('Content-Type', 'application/json');
    }
    if (token) {
      headers.set('Authorization', `Bearer ${token}`);
    }

    // When using Vite proxy, use relative paths
    const url = path.startsWith('/') ? path : `/${path}`;
    console.log('HTTP Request:', { url, method: options.method || 'GET', headers: Object.fromEntries(headers.entries()) });

    // Add timeout to fetch request
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 10000); // 10 second timeout
    
    let res;
    try {
      console.log('Making fetch request to:', url);
      res = await fetch(url, { ...options, headers, signal: controller.signal });
      clearTimeout(timeoutId);
      console.log('HTTP Response:', { status: res.status, statusText: res.statusText, headers: Object.fromEntries(res.headers.entries()) });
    } catch (fetchError) {
      clearTimeout(timeoutId);
      console.error('Fetch error details:', {
        name: fetchError.name,
        message: fetchError.message,
        cause: fetchError.cause
      });
      
      if (fetchError.name === 'AbortError') {
        throw new Error('Request timeout: Server took too long to respond');
      }
      
      if (fetchError.name === 'TypeError' && fetchError.message.includes('fetch')) {
        throw new Error('Network error: Unable to connect to server');
      }
      
      throw fetchError;
    }

    if (res.status === 204) return null;

    if (!res.ok) {
      const contentType = res.headers.get('content-type') || '';
      let data;
      try {
        data = contentType.includes('application/json') ? await res.json() : await res.text();
      } catch (parseError) {
        console.error('Failed to parse response:', parseError);
        data = await res.text();
      }
      
      console.log('HTTP Response Data:', data);
      
      if (res.status === 401) {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
        throw new Error('Unauthorized');
      }
      
      const msg = (data && (data.message || data.error)) || res.statusText;
      throw new Error(msg);
    }

    const contentType = res.headers.get('content-type') || '';
    const data = contentType.includes('application/json') ? await res.json() : await res.text();
    console.log('HTTP Response Data:', data);
    return data;
  } catch (error) {
    console.error('HTTP Error:', error);
    
    // Handle specific network errors
    if (error.name === 'TypeError' && error.message.includes('fetch')) {
      throw new Error('Network error: Unable to connect to server');
    }
    
    if (error.name === 'TypeError' && error.message.includes('Load failed')) {
      throw new Error('Network error: Request failed to load');
    }
    
    throw error;
  }
}