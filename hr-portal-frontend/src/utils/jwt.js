/**
 * Decode JWT token and extract payload
 * @param {string} token - JWT token
 * @returns {object|null} Decoded payload or null if invalid
 */
export function decodeJWT(token) {
  try {
    if (!token || typeof token !== 'string') {
      console.error('Invalid token format:', token);
      return null;
    }
    
    const parts = token.split('.');
    if (parts.length !== 3) {
      console.error('Invalid JWT format - expected 3 parts, got:', parts.length);
      return null;
    }
    
    console.log('JWT parts:', parts.map((part, i) => `Part ${i}: ${part.substring(0, 20)}...`));
    
    const base64Url = parts[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    
    console.log('Base64 decoded:', base64);
    
    try {
      const decoded = atob(base64);
      console.log('atob result length:', decoded.length);
      
      const jsonPayload = decodeURIComponent(decoded.split('').map(function(c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      console.log('JSON payload string:', jsonPayload);
      
      return JSON.parse(jsonPayload);
    } catch (decodeError) {
      console.error('Error in base64 decoding:', decodeError);
      
      // Fallback: try to decode without URI decoding
      try {
        const decoded = atob(base64);
        return JSON.parse(decoded);
      } catch (fallbackError) {
        console.error('Fallback decoding also failed:', fallbackError);
        return null;
      }
    }
  } catch (error) {
    console.error('Failed to decode JWT:', error);
    return null;
  }
}

/**
 * Extract user role from JWT token
 * @param {string} token - JWT token
 * @returns {string|null} User role or null if not found
 */
export function extractRoleFromToken(token) {
  const payload = decodeJWT(token);
  if (!payload) return null;
  
  // Debug: log the payload to see the structure
  console.log('JWT Payload:', payload);
  
  // Check for role in claims - adjust these based on your JWT structure
  // Common JWT claim names for roles in ASP.NET Core Identity
  const role = payload.role || payload.Role || 
               payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
               payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/name'] ||
               payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] ||
               payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
               null;
               
  console.log('Extracted role:', role);
  return role;
}

/**
 * Extract user ID from JWT token
 * @param {string} token - JWT token
 * @returns {string|null} User ID or null if not found
 */
export function extractUserIdFromToken(token) {
  const payload = decodeJWT(token);
  if (!payload) return null;
  
  // Check for user ID in claims - adjust these based on your JWT structure
  return payload.nameid || payload.NameId || payload.sub || payload.userId || null;
}

/**
 * Check if JWT token is expired
 * @param {string} token - JWT token
 * @returns {boolean} True if expired, false otherwise
 */
export function isTokenExpired(token) {
  const payload = decodeJWT(token);
  if (!payload) return true;
  
  const exp = payload.exp;
  if (!exp) return true;
  
  const currentTime = Math.floor(Date.now() / 1000);
  return currentTime >= exp;
}

/**
 * Get all claims from JWT token for debugging
 * @param {string} token - JWT token
 * @returns {object|null} All claims or null if invalid
 */
export function getAllClaims(token) {
  return decodeJWT(token);
}
