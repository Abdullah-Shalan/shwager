import { http } from './http.js';

/**
 * @typedef {import('../dtos.d.ts').JobSummaryDto} JobSummaryDto
 * @typedef {import('../dtos.d.ts').JobDto} JobDto
 * @typedef {import('../dtos.d.ts').JobResponse} JobResponse
 * @typedef {import('../dtos.d.ts').JobTaskDto} JobTaskDto
 * @typedef {import('../dtos.d.ts').TaskResponse} TaskResponse
 * @typedef {import('../dtos.d.ts').ReorderRequest} ReorderRequest
 * @typedef {import('../dtos.d.ts').CandidateProfileDto} CandidateProfileDto
 * @typedef {import('../dtos.d.ts').CandidateTaskDto} CandidateTaskDto
 */

// Jobs
/**
 * Get all available jobs
 * @returns {Promise<JobSummaryDto[]>} List of all jobs
 */
export const listJobs = () => http('/api/hr/jobs');

/**
 * Get job by ID
 * @param {number} jobId - ID of the job
 * @returns {Promise<JobSummaryDto>} Job details
 */
export const getJobById = (jobId) => http(`/api/hr/jobs/${jobId}`);

/**
 * Create a new job
 * @param {JobDto} jobDto - Job creation data
 * @returns {Promise<JobResponse>} Created job response
 */
export const createJob = (jobDto) => http('/api/hr/jobs', { method: 'POST', body: JSON.stringify(jobDto) });

/**
 * Edit an existing job
 * @param {number} jobId - ID of the job to edit
 * @param {JobDto} jobDto - Updated job data
 * @returns {Promise<void>}
 */
export const editJob = (jobId, jobDto) => http(`/api/hr/jobs/${jobId}`, { method: 'PUT', body: JSON.stringify(jobDto) });

/**
 * Delete a job
 * @param {number} jobId - ID of the job to delete
 * @returns {Promise<void>}
 */
export const deleteJob = (jobId) => http(`/api/hr/jobs/${jobId}`, { method: 'DELETE' });

// Job tasks
/**
 * Create a new task for a job
 * @param {number} jobId - ID of the job
 * @param {JobTaskDto} jobTaskDto - Task creation data
 * @returns {Promise<TaskResponse>} Created task response
 */
export const createJobTask = (jobId, jobTaskDto) =>
  http(`/api/hr/jobs/${jobId}/tasks`, { method: 'POST', body: JSON.stringify(jobTaskDto) });

/**
 * Get job task by ID
 * @param {number} jobTaskId - ID of the job task
 * @returns {Promise<TaskResponse>} Task details
 */
export const getJobTaskById = (jobTaskId) => http(`/api/hr/tasks/${jobTaskId}`);

/**
 * Get all tasks for a specific job
 * @param {number} jobId - ID of the job
 * @returns {Promise<JobTaskSummaryDto[]>} List of tasks for the job
 */
export const getJobTasksByJobId = (jobId) => http(`/api/hr/jobs/${jobId}/tasks`);

/**
 * Edit an existing job task
 * @param {number} jobTaskId - ID of the job task to edit
 * @param {JobTaskDto} jobTaskDto - Updated task data
 * @returns {Promise<void>}
 */
export const editJobTask = (jobTaskId, jobTaskDto) =>
  http(`/api/hr/tasks/${jobTaskId}`, { method: 'PUT', body: JSON.stringify(jobTaskDto) });

/**
 * Delete a job task
 * @param {number} jobTaskId - ID of the job task to delete
 * @returns {Promise<void>}
 */
export const deleteJobTask = (jobTaskId) => http(`/api/hr/tasks/${jobTaskId}`, { method: 'DELETE' });

/**
 * Reorder job tasks
 * @param {number} jobId - ID of the job
 * @param {ReorderRequest[]} newOrders - New task order configuration
 * @returns {Promise<void>}
 */
export const reorderJobTasks = (jobId, newOrders) =>
  http(`/api/hr/jobs/${jobId}/tasks/reorder`, { method: 'PUT', body: JSON.stringify(newOrders) });

/**
 * Set file requirement for a task
 * @param {number} jobTaskId - ID of the job task
 * @returns {Promise<void>}
 */
export const setTaskFileRequirement = (jobTaskId) =>
  http(`/api/hr/tasks/${jobTaskId}/set-file-requirement`, { method: 'PUT' });

/**
 * Set verification requirement for a task
 * @param {number} jobTaskId - ID of the job task
 * @returns {Promise<void>}
 */
export const setTaskVerificationRequirement = (jobTaskId) =>
  http(`/api/hr/tasks/${jobTaskId}/set-verification-requirement`, { method: 'PUT' });

// Candidates
/**
 * Get all candidates
 * @returns {Promise<CandidateProfileDto[]>} List of all candidates
 */
export const listCandidates = () => http('/api/hr/candidates');

/**
 * Get candidate profile by ID
 * @param {number} candidateId - ID of the candidate
 * @returns {Promise<CandidateProfileDto>} Candidate profile details
 */
export const getCandidateProfile = (candidateId) => http(`/api/hr/candidates/${candidateId}/profile`);

/**
 * Get candidate task progress
 * @param {number} candidateId - ID of the candidate
 * @returns {Promise<CandidateTaskDto[]>} List of candidate tasks with progress
 */
export const getCandidateTaskProgress = (candidateId) => http(`/api/hr/candidates/${candidateId}/tasks`);

/**
 * Verify a completed task
 * @param {number} candidateTaskId - ID of the candidate task
 * @returns {Promise<void>}
 */
export const verifyCompletedTask = (candidateTaskId) => http(`/api/hr/candidates/tasks/${candidateTaskId}/verify`, { method: 'PUT' });

/**
 * Delete a candidate
 * @param {number} candidateId - ID of the candidate to delete
 * @returns {Promise<void>}
 */
export const deleteCandidate = (candidateId) => http(`/api/hr/candidates/${candidateId}`, { method: 'DELETE' });