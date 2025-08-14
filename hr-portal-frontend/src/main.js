import { API_BASE_URL, TOKEN_STORAGE_KEY, USER_ROLE_KEY } from './config.js';
import { login, candidateRegister, hrRegister, logout, refreshUserRole } from './api/auth.js';
import { getAllClaims } from './utils/jwt.js';
import { runAllJWTTests } from './utils/jwt-test.js';
import { runAllHTTPTests } from './utils/http-test.js';
import * as cand from './api/candidate.js';
import * as hr from './api/hr.js';

/**
 * @typedef {import('./dtos.d.ts').JobSummaryDto} JobSummaryDto
 * @typedef {import('./dtos.d.ts').CandidateTaskDto} CandidateTaskDto
 * @typedef {import('./dtos.d.ts').CandidateProfileDto} CandidateProfileDto
 * @typedef {import('./dtos.d.ts').CandidateDto} CandidateDto
 * @typedef {import('./dtos.d.ts').JobDto} JobDto
 * @typedef {import('./dtos.d.ts').JobTaskDto} JobTaskDto
 * @typedef {import('./dtos.d.ts').ReorderRequest} ReorderRequest
 */

const q = (sel) => document.querySelector(sel);

function setAlert(el, text, type = 'info') {
  el.className = `alert alert-${type}`;
  el.textContent = text;
  el.classList.toggle('d-none', !text);
}

// Auth UI
/**
 * Handle login form submission
 * @param {Event} e - Form submit event
 */
async function handleLogin(e) {
  e.preventDefault();
  const email = q('#login-email').value.trim();
  const password = q('#login-password').value;
  const alert = q('#login-alert');
  
  if (!email || !password) {
    setAlert(alert, 'Please enter both email and password', 'warning');
    return;
  }
  
  setAlert(alert, 'Logging in...', 'info');
  
  try {
    console.log('Starting login process...');
    await login({ email, password });
    console.log('Login successful, updating UI...');
    
    renderAuthStatus();
    
    // Switch to appropriate tab based on role
    const role = localStorage.getItem(USER_ROLE_KEY);
    if (role) {
      const targetTab = role.toLowerCase() === 'candidate' ? '#candidate' : '#hr';
      const tabButton = document.querySelector(`[data-bs-target="${targetTab}"]`);
      if (tabButton) {
        tabButton.click();
      }
    }
    
    setAlert(alert, 'Logged in successfully!', 'success');
  } catch (err) {
    console.error('Login error in handleLogin:', err);
    const errorMessage = err.message || 'Login failed';
    setAlert(alert, `Login failed: ${errorMessage}`, 'danger');
  }
}

/**
 * Handle registration
 */
async function handleRegister() {
  const role = q('#reg-role').value;
  const alert = q('#reg-alert');
  setAlert(alert, '');
  
  try {
    let payload;
    
    if (role === 'candidate') {
      payload = {
        firstName: q('#cand-reg-firstname').value.trim(),
        lastName: q('#cand-reg-lastname').value.trim(),
        email: q('#cand-reg-email').value.trim(),
        password: q('#cand-reg-password').value,
        resumeUrl: q('#cand-reg-resume').value.trim()
      };
      
      // Validate required fields
      if (!payload.firstName || !payload.lastName || !payload.email || !payload.password || !payload.resumeUrl) {
        setAlert(alert, 'Please fill in all required fields', 'warning');
        return;
      }
      
      const res = await candidateRegister(payload);
      setAlert(alert, `Candidate registered successfully: ${res.firstName} ${res.lastName}`, 'success');
      
      // Clear form
      q('#candidate-register-form').reset();
      
    } else if (role === 'hr') {
      payload = {
        firstName: q('#hr-reg-firstname').value.trim(),
        lastName: q('#hr-reg-lastname').value.trim(),
        email: q('#hr-reg-email').value.trim(),
        password: q('#hr-reg-password').value
      };
      
      // Validate required fields
      if (!payload.firstName || !payload.lastName || !payload.email || !payload.password) {
        setAlert(alert, 'Please fill in all required fields', 'warning');
        return;
      }
      
      const res = await hrRegister(payload);
      setAlert(alert, `HR registered successfully: ${res.firstName} ${res.lastName}`, 'success');
      
      // Clear form
      q('#hr-register-form').reset();
    }
  } catch (err) {
    setAlert(alert, err.message || 'Register failed', 'danger');
  }
}

function renderAuthStatus() {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  const role = localStorage.getItem(USER_ROLE_KEY);
  q('#auth-status').textContent = token ? `Role: ${role || 'unknown'} | API: ${API_BASE_URL}` : 'Not authenticated';
  q('#logout-btn').classList.toggle('d-none', !token);
  q('#debug-jwt-btn').classList.toggle('d-none', !token);
  q('#refresh-role-btn').classList.toggle('d-none', !token);
  
  // Show/hide tabs based on user role
  const candidateTab = document.querySelector('[data-bs-target="#candidate"]');
  const hrTab = document.querySelector('[data-bs-target="#hr"]');
  
  if (token && role) {
    if (role.toLowerCase() === 'candidate') {
      candidateTab?.classList.remove('d-none');
      hrTab?.classList.add('d-none');
    } else if (role.toLowerCase() === 'hr') {
      candidateTab?.classList.add('d-none');
      hrTab?.classList.remove('d-none');
    } else {
      // Show both tabs for unknown roles
      candidateTab?.classList.remove('d-none');
      hrTab?.classList.remove('d-none');
    }
  } else {
    // Hide both tabs when not authenticated
    candidateTab?.classList.add('d-none');
    hrTab?.classList.add('d-none');
  }
}

// Candidate UI
async function renderCandidate() {
  await Promise.all([renderCandAvailableJobs(), renderCandAssignedJob(), renderCandTasks(), renderCandProfile()]);
}

async function renderCandAvailableJobs() {
  const tbody = q('#cand-available-tbody');
  tbody.innerHTML = '';
  try {
    /** @type {JobSummaryDto[]} */
    const jobs = await cand.getAvailableJobs();
    for (const j of jobs || []) {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${j.id ?? ''}</td>
        <td>${j.title ?? ''}</td>
        <td class="text-end">
          <button class="btn btn-sm btn-outline-primary">Assign</button>
        </td>
      `;
      tr.querySelector('button').addEventListener('click', async () => {
        try { await cand.assignToJob(j.id); await renderCandidate(); } catch (e) { alert(e.message); }
      });
      tbody.appendChild(tr);
    }
  } catch (e) {
    tbody.innerHTML = `<tr><td colspan="3" class="text-danger">${e.message}</td></tr>`;
  }
}

async function renderCandAssignedJob() {
  const pre = q('#cand-assigned-job');
  try {
    /** @type {JobSummaryDto} */
    const job = await cand.getAssignedJob();
    pre.textContent = JSON.stringify(job ?? {}, null, 2);
  } catch (e) {
    pre.textContent = e.message;
  }
}

async function renderCandTasks() {
  const tbody = q('#cand-tasks-tbody');
  tbody.innerHTML = '';
  try {
    /** @type {CandidateTaskDto[]} */
    const tasks = await cand.getTasks();
    for (const t of tasks || []) {
      const tr = document.createElement('tr');
      const status = t.status === 2 || t.status === 'Completed' ? 'Completed' : 'Pending';
      const isCompleted = t.status === 2 || t.status === 'Completed';
      tr.innerHTML = `
        <td>${t.id ?? ''}</td>
        <td>${t.description ?? ''}</td>
        <td>${status}</td>
        <td class="text-end">
          <button class="btn btn-sm ${isCompleted ? 'btn-success' : 'btn-outline-success'}" ${isCompleted ? 'disabled' : ''}>Complete</button>
        </td>
      `;
      tr.querySelector('button').addEventListener('click', async () => {
        try { await cand.completeTask(t.id); await renderCandTasks(); } catch (e) { alert(e.message); }
      });
      tbody.appendChild(tr);
    }
  } catch (e) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-danger">${e.message}</td></tr>`;
  }
}

async function renderCandProfile() {
  try {
    /** @type {CandidateProfileDto} */
    const profile = await cand.viewProfile();
    
    if (profile) {
      q('#cand-profile-firstname').value = profile.firstName || '';
      q('#cand-profile-lastname').value = profile.lastName || '';
    }
  } catch (e) {
    console.error('Failed to load profile:', e);
    // Clear form on error
    q('#cand-profile-firstname').value = '';
    q('#cand-profile-lastname').value = '';
  }
}

// HR UI
async function renderHR() {
  await Promise.all([renderHRJobs(), renderHRCandidates(), renderHRAvailableJobs()]);
}

async function renderHRJobs() {
  const tbody = q('#hr-jobs-tbody');
  tbody.innerHTML = '';
  try {
    /** @type {JobSummaryDto[]} */
    const jobs = await hr.listJobs();
    for (const j of jobs || []) {
      const tr = document.createElement('tr');
      tr.innerHTML = `
        <td>${j.id ?? ''}</td>
        <td>${j.title ?? ''}</td>
        <td class="text-end">
          <button class="btn btn-sm btn-outline-danger">Delete</button>
        </td>
      `;
      tr.querySelector('button').addEventListener('click', async () => {
        if (!confirm('Delete job?')) return;
        try { await hr.deleteJob(j.id); await renderHRJobs(); } catch (e) { alert(e.message); }
      });
      tbody.appendChild(tr);
    }
  } catch (e) {
    tbody.innerHTML = `<tr><td colspan="3" class="text-danger">${e.message}</td></tr>`;
  }
}

async function renderHRCandidates() {
  const tbody = q('#hr-cands-tbody');
  const pre = q('#hr-selected-cand');
  tbody.innerHTML = '';
  pre.textContent = '';
  try {
    /** @type {CandidateProfileDto[]} */
    const cands = await hr.listCandidates();
    for (const c of cands || []) {
      const tr = document.createElement('tr');
      const name = `${c.firstName ?? ''} ${c.lastName ?? ''}`.trim();
      tr.innerHTML = `
        <td>${c.id ?? ''}</td>
        <td>${name || '(no name)'}</td>
        <td class="text-end">
          <div class="btn-group btn-group-sm">
            <button class="btn btn-outline-secondary">View</button>
            <button class="btn btn-outline-danger">Delete</button>
          </div>
        </td>
      `;
      const [viewBtn, delBtn] = tr.querySelectorAll('button');
      viewBtn.addEventListener('click', async () => {
        try {
          /** @type {CandidateProfileDto} */
          const profile = await hr.getCandidateProfile(c.id);
          /** @type {CandidateTaskDto[]} */
          const tasks = await hr.getCandidateTaskProgress(c.id);
          pre.textContent = JSON.stringify({ profile, tasks }, null, 2);
        } catch (e) { pre.textContent = e.message; }
      });
      delBtn.addEventListener('click', async () => {
        if (!confirm('Delete candidate?')) return;
        try { await hr.deleteCandidate(c.id); await renderHRCandidates(); } catch (e) { alert(e.message); }
      });
      tbody.appendChild(tr);
    }
  } catch (e) {
    tbody.innerHTML = `<tr><td colspan="3" class="text-danger">${e.message}</td></tr>`;
  }
}

async function renderHRAvailableJobs() {
  const tbody = q('#hr-available-jobs-tbody');
  const alert = q('#hr-available-jobs-alert');
  tbody.innerHTML = '';
  setAlert(alert, 'Loading available jobs...', 'info');
  
  try {
    /** @type {JobSummaryDto[]} */
    const jobs = await hr.listJobs();
    
    if (!jobs || jobs.length === 0) {
      tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">No available jobs found</td></tr>`;
      setAlert(alert, 'No available jobs found', 'info');
      return;
    }
    
    for (const job of jobs) {
      const tr = document.createElement('tr');
      const tasksCount = job.jobTasks ? job.jobTasks.length : 0;
      
      tr.innerHTML = `
        <td>${job.id ?? ''}</td>
        <td>${job.title ?? ''}</td>
        <td>${(job.description ?? '').substring(0, 100)}${(job.description ?? '').length > 100 ? '...' : ''}</td>
        <td>${tasksCount}</td>
        <td class="text-end">
          <div class="btn-group btn-group-sm">
            <button class="btn btn-outline-primary btn-view-job">View Details</button>
            <button class="btn btn-outline-success btn-manage-tasks">Manage Tasks</button>
          </div>
        </td>
      `;
      
      // Add event listeners for the action buttons
      const viewBtn = tr.querySelector('.btn-view-job');
      const manageBtn = tr.querySelector('.btn-manage-tasks');
      
      viewBtn.addEventListener('click', async () => {
        try {
          // Show loading state
          viewBtn.disabled = true;
          viewBtn.textContent = 'Loading...';
          
          // Fetch job details and tasks
          const jobDetails = await hr.getJobById(job.id);
          const jobTasks = await hr.getJobTasksByJobId(job.id);
          
          // Create detailed view
          let detailsHtml = `
            <div class="modal fade" id="jobDetailsModal" tabindex="-1">
              <div class="modal-dialog modal-lg">
                <div class="modal-content">
                  <div class="modal-header">
                    <h5 class="modal-title">Job Details: ${job.title}</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                  </div>
                  <div class="modal-body">
                    <div class="row mb-3">
                      <div class="col-md-6">
                        <strong>Job ID:</strong> ${job.id}
                      </div>
                      <div class="col-md-6">
                        <strong>Total Tasks:</strong> ${tasksCount}
                      </div>
                    </div>
                    <div class="mb-3">
                      <strong>Description:</strong>
                      <p class="mt-1">${job.description || 'No description available'}</p>
                    </div>
                    <div class="mb-3">
                      <strong>Tasks:</strong>
                      <div class="table-responsive mt-2">
                        <table class="table table-sm">
                          <thead>
                            <tr>
                              <th>Order</th>
                              <th>Description</th>
                              <th>File Required</th>
                              <th>Verification Required</th>
                            </tr>
                          </thead>
                          <tbody>`;
          
          if (jobTasks && jobTasks.length > 0) {
            jobTasks.forEach(task => {
              detailsHtml += `
                <tr>
                  <td>${task.order || 'N/A'}</td>
                  <td>${task.description || 'N/A'}</td>
                  <td><span class="badge ${task.requiresFile ? 'bg-success' : 'bg-secondary'}">${task.requiresFile ? 'Yes' : 'No'}</span></td>
                  <td><span class="badge ${task.requiresVerification ? 'bg-warning' : 'bg-secondary'}">${task.requiresVerification ? 'Yes' : 'No'}</span></td>
                </tr>`;
            });
          } else {
            detailsHtml += `
              <tr>
                <td colspan="4" class="text-center text-muted">No tasks found for this job</td>
              </tr>`;
          }
          
          detailsHtml += `
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" onclick="document.getElementById('hr-task-job-id').value='${job.id}'; bootstrap.Modal.getInstance(document.getElementById('jobDetailsModal')).hide();">Create Task for This Job</button>
                  </div>
                </div>
              </div>
            </div>`;
          
          // Remove existing modal if any
          const existingModal = document.getElementById('jobDetailsModal');
          if (existingModal) {
            existingModal.remove();
          }
          
          // Add modal to DOM
          document.body.insertAdjacentHTML('beforeend', detailsHtml);
          
          // Show modal
          const modal = new bootstrap.Modal(document.getElementById('jobDetailsModal'));
          modal.show();
          
        } catch (error) {
          console.error('Error loading job details:', error);
          alert(`Failed to load job details: ${error.message}`);
        } finally {
          // Reset button state
          viewBtn.disabled = false;
          viewBtn.textContent = 'View Details';
        }
      });
      
      manageBtn.addEventListener('click', () => {
        // Auto-fill the job ID in the task creation form
        q('#hr-task-job-id').value = job.id;
        // Scroll to the task management section
        document.querySelector('[data-bs-target="#hr"]').click();
        // You could also add a smooth scroll to the task section
      });
      
      tbody.appendChild(tr);
    }
    
    setAlert(alert, `Loaded ${jobs.length} available jobs successfully`, 'success');
  } catch (e) {
    console.error('Error loading available jobs:', e);
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger">Failed to load jobs: ${e.message}</td></tr>`;
    setAlert(alert, `Failed to load jobs: ${e.message}`, 'danger');
  }
}

// Wire events
q('#login-form').addEventListener('submit', handleLogin);
q('#register-btn').addEventListener('click', handleRegister);

// Show/hide registration forms based on role selection
q('#reg-role').addEventListener('change', () => {
  const role = q('#reg-role').value;
  const candidateForm = q('#candidate-register-form');
  const hrForm = q('#hr-register-form');
  
  if (role === 'candidate') {
    candidateForm.classList.remove('d-none');
    hrForm.classList.add('d-none');
  } else if (role === 'hr') {
    candidateForm.classList.add('d-none');
    hrForm.classList.remove('d-none');
  }
});

// Initialize form visibility
q('#reg-role').dispatchEvent(new Event('change'));
q('#logout-btn').addEventListener('click', () => { 
  logout(); 
  renderAuthStatus(); 
  
  // Switch back to auth tab after logout
  const authTab = document.querySelector('[data-bs-target="#auth"]');
  if (authTab) {
    authTab.click();
  }
});

q('#refresh-role-btn').addEventListener('click', () => {
  const role = refreshUserRole();
  if (role) {
    renderAuthStatus();
    alert(`Role refreshed: ${role}`);
  } else {
    alert('Failed to refresh role. Please login again.');
  }
});

q('#debug-jwt-btn').addEventListener('click', () => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (token) {
    const claims = getAllClaims(token);
    if (claims) {
      console.log('JWT Claims:', claims);
      alert(`JWT Claims logged to console. Check browser console for details.\n\nRole: ${claims.role || claims.Role || 'Not found'}`);
    } else {
      alert('Failed to decode JWT token');
    }
  } else {
    alert('No JWT token found');
  }
});

q('#test-jwt-btn').addEventListener('click', () => {
  const result = runAllJWTTests();
  if (result) {
    alert('All JWT tests passed! Check console for details.');
  } else {
    alert('Some JWT tests failed. Check console for details.');
  }
});

q('#test-http-btn').addEventListener('click', async () => {
  try {
    const result = await runAllHTTPTests();
    if (result) {
      alert('All HTTP tests passed! Check console for details.');
    } else {
      alert('Some HTTP tests failed. Check console for details.');
    }
  } catch (error) {
    alert(`HTTP test error: ${error.message}`);
    console.error('HTTP test error:', error);
  }
});

q('#cand-refresh-profile').addEventListener('click', renderCandProfile);
q('#cand-save-profile').addEventListener('click', async () => {
  const alert = q('#cand-alert');
  try {
    const dto = {
      firstName: q('#cand-profile-firstname').value.trim(),
      lastName: q('#cand-profile-lastname').value.trim()
    };
    
    // Validate required fields
    if (!dto.firstName || !dto.lastName) {
      setAlert(alert, 'Please fill in both first name and last name', 'warning');
      return;
    }
    
    await cand.editProfile(dto);
    setAlert(alert, 'Profile saved successfully', 'success');
  } catch (e) {
    setAlert(alert, e.message, 'danger');
  }
});
q('#upload-resume-btn').addEventListener('click', async () => {
  const file = q('#resume-file').files[0];
  const alert = q('#cand-alert');
  if (!file) return setAlert(alert, 'Choose a file first', 'warning');
  try {
    await cand.uploadResume(file);
    setAlert(alert, 'Resume uploaded', 'success');
  } catch (e) {
    setAlert(alert, e.message, 'danger');
  }
});

q('#hr-refresh-jobs').addEventListener('click', renderHRJobs);
q('#hr-create-job').addEventListener('click', async () => {
  try {
    const dto = {
      title: q('#hr-job-title').value.trim(),
      description: q('#hr-job-description').value.trim()
    };
    
    // Validate required fields
    if (!dto.title || !dto.description) {
      alert('Please fill in both job title and description');
      return;
    }
    
    console.log('Creating job with DTO:', dto);
    const result = await hr.createJob(dto);
    console.log('Job created successfully:', result);
    
    await renderHRJobs();
    
    // Clear form
    q('#hr-job-form').reset();
    alert('Job created successfully!');
  } catch (e) { 
    console.error('Error creating job:', e);
    alert(`Failed to create job: ${e.message}`); 
  }
});

q('#hr-edit-job').addEventListener('click', async () => {
  try {
    const id = Number(q('#hr-edit-job-id').value);
    
    if (!id) {
      alert('Please enter Job ID');
      return;
    }
    
    console.log('Loading job for editing:', id);
    const job = await hr.getJobById(id);
    
    if (job) {
      q('#hr-edit-job-title').value = job.title || '';
      q('#hr-edit-job-description').value = job.description || '';
      alert('Job loaded for editing. Make your changes and click "Save Changes".');
    } else {
      alert('Job not found');
    }
  } catch (e) { 
    console.error('Error loading job:', e);
    alert(`Failed to load job: ${e.message}`); 
  }
});

q('#hr-save-edit-job').addEventListener('click', async () => {
  try {
    const id = Number(q('#hr-edit-job-id').value);
    const dto = {
      title: q('#hr-edit-job-title').value.trim(),
      description: q('#hr-edit-job-description').value.trim()
    };
    
    if (!id) {
      alert('Please enter Job ID');
      return;
    }
    
    // Validate required fields
    if (!dto.title || !dto.description) {
      alert('Please fill in both job title and description');
      return;
    }
    
    console.log('Saving job changes:', { id, dto });
    await hr.editJob(id, dto);
    await renderHRJobs();
    
    // Clear form
    q('#hr-edit-job-id').value = '';
    q('#hr-edit-job-form').reset();
    alert('Job updated successfully!');
  } catch (e) { 
    console.error('Error updating job:', e);
    alert(`Failed to update job: ${e.message}`); 
  }
});
q('#hr-refresh-candidates').addEventListener('click', renderHRCandidates);
q('#hr-refresh-available-jobs').addEventListener('click', renderHRAvailableJobs);

// Job Task Management
q('#hr-create-task').addEventListener('click', async () => {
  try {
    const jobId = Number(q('#hr-task-job-id').value);
    const dto = {
      description: q('#hr-task-description').value.trim(),
      requiresFile: q('#hr-task-requires-file').checked,
      requiresVerification: q('#hr-task-requires-verification').checked
    };
    
    if (!jobId) {
      alert('Please enter Job ID');
      return;
    }
    
    if (!dto.description) {
      alert('Please enter task description');
      return;
    }
    
    console.log('Creating task with DTO:', { jobId, dto });
    console.log('Task DTO details:', {
      description: dto.description,
      requiresFile: dto.requiresFile,
      requiresVerification: dto.requiresVerification,
      descriptionType: typeof dto.description,
      requiresFileType: typeof dto.requiresFile,
      requiresVerificationType: typeof dto.requiresVerification
    });
    
    const result = await hr.createJobTask(jobId, dto);
    console.log('Task created successfully:', result);
    
    await renderHRJobs();
    
    // Clear form
    q('#hr-create-task-form').reset();
    alert('Task created successfully!');
  } catch (e) { 
    console.error('Error creating task:', e);
    alert(`Failed to create task: ${e.message}`); 
  }
});

q('#hr-load-task').addEventListener('click', async () => {
  try {
    const taskId = Number(q('#hr-edit-task-id').value);
    
    if (!taskId) {
      alert('Please enter Task ID');
      return;
    }
    
    console.log('Loading task for editing:', taskId);
    const task = await hr.getJobTaskById(taskId);
    
    if (task) {
      q('#hr-edit-task-description').value = task.task || '';
      q('#hr-edit-task-requires-file').checked = task.requiresFile || false;
      q('#hr-edit-task-requires-verification').checked = task.requiresVerification || false;
      alert('Task loaded for editing. Make your changes and click "Save Changes".');
    } else {
      alert('Task not found');
    }
  } catch (e) { 
    console.error('Error loading task:', e);
    alert(`Failed to load task: ${e.message}`); 
  }
});

q('#hr-save-edit-task').addEventListener('click', async () => {
  try {
    const taskId = Number(q('#hr-edit-task-id').value);
    const dto = {
      description: q('#hr-edit-task-description').value.trim(),
      requiresFile: q('#hr-edit-task-requires-file').checked,
      requiresVerification: q('#hr-edit-task-requires-verification').checked
    };
    
    if (!taskId) {
      alert('Please enter Task ID');
      return;
    }
    
    if (!dto.description) {
      alert('Please enter task description');
      return;
    }
    
    console.log('Saving task changes:', { taskId, dto });
    await hr.editJobTask(taskId, dto);
    await renderHRJobs();
    
    // Clear form
    q('#hr-edit-task-id').value = '';
    q('#hr-edit-task-form').reset();
    alert('Task updated successfully!');
  } catch (e) { 
    console.error('Error updating task:', e);
    alert(`Failed to update task: ${e.message}`); 
  }
});

// Add tab change event listeners
document.addEventListener('DOMContentLoaded', () => {
  const candidateTab = document.querySelector('[data-bs-target="#candidate"]');
  const hrTab = document.querySelector('[data-bs-target="#hr"]');
  
  candidateTab?.addEventListener('shown.bs.tab', () => {
    if (localStorage.getItem(TOKEN_STORAGE_KEY)) {
      renderCandidate();
    }
  });
  
  hrTab?.addEventListener('shown.bs.tab', () => {
    if (localStorage.getItem(TOKEN_STORAGE_KEY)) {
      renderHR();
    }
  });
});

// Global error handling
window.addEventListener('error', (event) => {
  console.error('Global error:', event.error);
  console.error('Error details:', {
    message: event.error?.message,
    stack: event.error?.stack,
    filename: event.filename,
    lineno: event.lineno,
    colno: event.colno
  });
});

window.addEventListener('unhandledrejection', (event) => {
  console.error('Unhandled promise rejection:', event.reason);
  console.error('Promise rejection details:', {
    reason: event.reason,
    stack: event.reason?.stack
  });
});

// Initial render
renderAuthStatus();