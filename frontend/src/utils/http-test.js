/**
 * Test HTTP utility with simple requests
 */
export async function testHTTPConnection() {
  console.log('=== Testing HTTP Connection ===');
  
  try {
    // Test 1: Simple GET request to a known endpoint
    console.log('Test 1: Testing GET request to /api/auth/login');
    
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email: 'test', password: 'test' })
    });
    
    console.log('Response status:', response.status);
    console.log('Response headers:', Object.fromEntries(response.headers.entries()));
    
    const text = await response.text();
    console.log('Response body:', text);
    
    return true;
  } catch (error) {
    console.error('HTTP test failed:', error);
    console.error('Error details:', {
      name: error.name,
      message: error.message,
      stack: error.stack
    });
    return false;
  }
}

/**
 * Test if the backend is accessible
 */
export async function testBackendAccess() {
  console.log('=== Testing Backend Access ===');
  
  try {
    // Test direct backend access
    const response = await fetch('http://localhost:5183/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email: 'test', password: 'test' })
    });
    
    console.log('Direct backend response status:', response.status);
    console.log('Direct backend response headers:', Object.fromEntries(response.headers.entries()));
    
    const text = await response.text();
    console.log('Direct backend response body:', text);
    
    return true;
  } catch (error) {
    console.error('Direct backend test failed:', error);
    console.error('Error details:', {
      name: error.name,
      message: error.message,
      stack: error.stack
    });
    return false;
  }
}

/**
 * Test Vite proxy functionality
 */
export async function testViteProxy() {
  console.log('=== Testing Vite Proxy ===');
  
  try {
    // Test proxy request
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email: 'test', password: 'test' })
    });
    
    console.log('Proxy response status:', response.status);
    console.log('Proxy response headers:', Object.fromEntries(response.headers.entries()));
    
    const text = await response.text();
    console.log('Proxy response body:', text);
    
    return true;
  } catch (error) {
    console.error('Proxy test failed:', error);
    console.error('Error details:', {
      name: error.name,
      message: error.message,
      stack: error.stack
    });
    return false;
  }
}

/**
 * Run all HTTP tests
 */
export async function runAllHTTPTests() {
  console.log('=== Running All HTTP Tests ===');
  
  const httpTest = await testHTTPConnection();
  const backendTest = await testBackendAccess();
  const proxyTest = await testViteProxy();
  
  console.log('=== HTTP Test Results ===');
  console.log('HTTP Connection:', httpTest ? 'PASS' : 'FAIL');
  console.log('Backend Access:', backendTest ? 'PASS' : 'FAIL');
  console.log('Vite Proxy:', proxyTest ? 'PASS' : 'FAIL');
  
  return httpTest && backendTest && proxyTest;
}
