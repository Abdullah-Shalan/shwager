import { http } from './http.js';
import { TOKEN_STORAGE_KEY, USER_ROLE_KEY } from '../config.js';
import { extractRoleFromToken, decodeJWT } from '../utils/jwt.js';

/**
 * @typedef {import('../dtos').LoginRequest} LoginRequest
 * @typedef {import('../dtos').CandidateRegisterRequest} CandidateRegisterRequest
 * @typedef {import('../dtos').CandidateRegisterResponse} CandidateRegisterResponse
 * @typedef {import('../dtos').HrRegisterRequest} HrRegisterRequest
 * @typedef {import('../dtos').HrRegisterResponse} HrRegisterResponse
 */

/**
 * Login with credentials
 * @param {LoginRequest} credentials - Login credentials
 * @returns {Promise<string>} JWT token
 */
export async function login(credentials) {
  try {
    
    console.log('Attempting login with credentials:', { email: credentials.email, password: '***' });
    
    const tokenResponse = await http('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    });
    
    console.log('Login response received:', tokenResponse);
    
    const token = typeof tokenResponse === 'string'
      ? tokenResponse
      : tokenResponse.token || tokenResponse.accessToken || '';
      
    if (!token) {
      console.error('No token in response:', tokenResponse);
      throw new Error('No token returned from server');
    }
    
    console.log('Token extracted:', token.substring(0, 50) + '...');
    
    // Extract role from JWT token
    const role = extractRoleFromToken(token);
    console.log('Role extracted:', role);
    
    if (!role) {
      const decodedToken = decodeJWT(token);
      console.error('JWT token structure:', decodedToken);
      throw new Error('No role found in JWT token. Please check the token structure.');
    }
    
    localStorage.setItem(TOKEN_STORAGE_KEY, token);
    localStorage.setItem(USER_ROLE_KEY, role);
    
    console.log('Login successful, role:', role);
    return token;
  } catch (error) {
    console.error('Login error:', error);
    throw error;
  }
}

/**
 * Register a new candidate
 * @param {CandidateRegisterRequest} payload - Candidate registration data
 * @returns {Promise<CandidateRegisterResponse>} Registration response
 */
export async function candidateRegister(payload) {
  return http('/api/auth/candidate/register', { method: 'POST', body: JSON.stringify(payload) });
}

/**
 * Register a new HR user
 * @param {HrRegisterRequest} payload - HR registration data
 * @returns {Promise<HrRegisterResponse>} Registration response
 */
export async function hrRegister(payload) {
  return http('/api/auth/hr/register', { method: 'POST', body: JSON.stringify(payload) });
}

export function logout() {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
  localStorage.removeItem(USER_ROLE_KEY);
}

/**
 * Get current user's role
 * @returns {string|null} User role or null if not authenticated
 */
export function getCurrentUserRole() {
  return localStorage.getItem(USER_ROLE_KEY);
}

/**
 * Check if user is authenticated
 * @returns {boolean} True if authenticated, false otherwise
 */
export function isAuthenticated() {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  return !!token;
}

/**
 * Check if current user has a specific role
 * @param {string} role - Role to check
 * @returns {boolean} True if user has the role, false otherwise
 */
export function hasRole(role) {
  const currentRole = getCurrentUserRole();
  return currentRole && currentRole.toLowerCase() === role.toLowerCase();
}

/**
 * Refresh user role from current JWT token
 * @returns {string|null} Updated role or null if failed
 */
export function refreshUserRole() {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (!token) return null;
  
  const role = extractRoleFromToken(token);
  if (role) {
    localStorage.setItem(USER_ROLE_KEY, role);
  }
  return role;
}