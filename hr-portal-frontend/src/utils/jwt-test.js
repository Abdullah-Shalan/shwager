import { decodeJWT } from './jwt.js';

/**
 * Test JWT decoding with a sample token
 * This helps debug JWT decoding issues
 */
export function testJWTDecoding() {
  console.log('Testing JWT decoding...');
  
  // Test with a simple JWT-like string
  const testToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
  
  try {
    console.log('Testing with sample JWT token...');
    const decoded = decodeJWT(testToken);
    console.log('Sample JWT decoded successfully:', decoded);
    return true;
  } catch (error) {
    console.error('Sample JWT decoding failed:', error);
    return false;
  }
}

/**
 * Test browser's atob function
 */
export function testAtob() {
  console.log('Testing browser atob function...');
  
  try {
    const testString = 'SGVsbG8gV29ybGQ='; // "Hello World" in base64
    const decoded = atob(testString);
    console.log('atob test successful:', decoded);
    return true;
  } catch (error) {
    console.error('atob test failed:', error);
    return false;
  }
}

/**
 * Test browser's decodeURIComponent function
 */
export function testDecodeURIComponent() {
  console.log('Testing browser decodeURIComponent function...');
  
  try {
    const testString = '%48%65%6c%6c%6f%20%57%6f%72%6c%64'; // "Hello World" in URI encoding
    const decoded = decodeURIComponent(testString);
    console.log('decodeURIComponent test successful:', decoded);
    return true;
  } catch (error) {
    console.error('decodeURIComponent test failed:', error);
    return false;
  }
}

/**
 * Run all JWT tests
 */
export function runAllJWTTests() {
  console.log('=== Running JWT Tests ===');
  
  const atobTest = testAtob();
  const decodeURITest = testDecodeURIComponent();
  const jwtTest = testJWTDecoding();
  
  console.log('=== Test Results ===');
  console.log('atob:', atobTest ? 'PASS' : 'FAIL');
  console.log('decodeURIComponent:', decodeURITest ? 'PASS' : 'FAIL');
  console.log('JWT Decoding:', jwtTest ? 'PASS' : 'FAIL');
  
  return atobTest && decodeURITest && jwtTest;
}
