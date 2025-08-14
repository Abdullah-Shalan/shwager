import { http } from './http.js';

/**
 * @typedef {import('../dtos').JobSummaryDto} JobSummaryDto
 * @typedef {import('../dtos').CandidateTaskDto} CandidateTaskDto
 * @typedef {import('../dtos').CandidateProfileDto} CandidateProfileDto
 * @typedef {import('../dtos').CandidateDto} CandidateDto
 */

/**
 * Get available jobs for candidates
 * @returns {Promise<JobSummaryDto[]>} List of available jobs
 */
export const getAvailableJobs = () => http('/api/candidate/available-jobs');

/**
 * Get the job assigned to the current candidate
 * @returns {Promise<JobSummaryDto>} Assigned job details
 */
export const getAssignedJob = () => http('/api/candidate/assigned-job');

/**
 * Assign candidate to a specific job
 * @param {number} jobId - ID of the job to assign
 * @returns {Promise<void>}
 */
export const assignToJob = (jobId) => http(`/api/candidate/jobs/${jobId}/assign`, { method: 'POST' });

/**
 * Get tasks assigned to the current candidate
 * @returns {Promise<CandidateTaskDto[]>} List of candidate tasks
 */
export const getAssignedTasks = () => http('/api/candidate/tasks');

/**
 * Get completion timestamp for a specific task
 * @param {number} candidateTaskId - ID of the candidate task
 * @returns {Promise<Date | null>} Completion timestamp or null if not completed
 */
export const getCompletionTimestamp = (candidateTaskId) =>
  http(`/api/candidate/tasks/${candidateTaskId}/completion-timestamp`);

/**
 * Mark a task as completed
 * @param {number} candidateTaskId - ID of the candidate task
 * @returns {Promise<void>}
 */
export const completeTask = (candidateTaskId) =>
  http(`/api/candidate/tasks/${candidateTaskId}/complete`, { method: 'POST' });

/**
 * View candidate profile
 * @returns {Promise<CandidateProfileDto>} Candidate profile information
 */
export const viewProfile = () => http('/api/candidate/view-profile');

/**
 * Edit candidate profile information
 * @param {CandidateDto} candidateDto - Updated candidate information
 * @returns {Promise<void>}
 */
export const editProfileInfo = (candidateDto) =>
  http('/api/candidate/profile', { method: 'PUT', body: JSON.stringify(candidateDto) });

/**
 * Upload candidate resume
 * @param {File} file - Resume file to upload
 * @returns {Promise<void>}
 */
export const uploadResume = (file) => {
  const form = new FormData();
  form.append('resume', file);
  return http('/api/candidate/upload-resume', { method: 'POST', body: form });
};

// Alias for backward compatibility
export const getTasks = getAssignedTasks;
export const editProfile = editProfileInfo;