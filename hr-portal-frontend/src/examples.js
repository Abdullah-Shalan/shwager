/**
 * Examples of how to use the DTOs with the API functions
 * This file demonstrates proper usage of the DTOs for all API calls
 */

import { login, candidateRegister, hrRegister } from './api/auth.js';
import * as candidateAPI from './api/candidate.js';
import * as hrAPI from './api/hr.js';

/**
 * Example: Login with proper DTO
 */
export async function exampleLogin() {
  try {
    // Using LoginRequest DTO
    const loginData = {
      email: "user@example.com",
      password: "P@ssw0rd"
    };
    
    const token = await login(loginData, 'candidate');
    console.log('Login successful, token:', token);
    return token;
  } catch (error) {
    console.error('Login failed:', error.message);
  }
}

/**
 * Example: Register candidate with proper DTO
 */
export async function exampleCandidateRegister() {
  try {
    // Using CandidateRegisterRequest DTO
    const candidateData = {
      firstName: "John",
      lastName: "Doe",
      email: "john.doe@example.com",
      password: "P@ssw0rd",
      resumeUrl: "https://example.com/resume.pdf"
    };
    
    const response = await candidateRegister(candidateData);
    console.log('Candidate registered:', response);
    return response;
  } catch (error) {
    console.error('Candidate registration failed:', error.message);
  }
}

/**
 * Example: Register HR with proper DTO
 */
export async function exampleHrRegister() {
  try {
    // Using HrRegisterRequest DTO
    const hrData = {
      firstName: "Jane",
      lastName: "Smith",
      email: "jane.smith@example.com",
      password: "P@ssw0rd"
    };
    
    const response = await hrRegister(hrData);
    console.log('HR registered:', response);
    return response;
  } catch (error) {
    console.error('HR registration failed:', error.message);
  }
}

/**
 * Example: Get available jobs (returns JobSummaryDto[])
 */
export async function exampleGetAvailableJobs() {
  try {
    const jobs = await candidateAPI.getAvailableJobs();
    console.log('Available jobs:', jobs);
    
    // Each job has JobSummaryDto structure:
    // { id: number, title: string, description: string, jobTasks?: JobTaskSummaryDto[] }
    
    jobs.forEach(job => {
      console.log(`Job ${job.id}: ${job.title}`);
      if (job.jobTasks) {
        job.jobTasks.forEach(task => {
          console.log(`  Task ${task.taskOrder}: ${task.description}`);
        });
      }
    });
    
    return jobs;
  } catch (error) {
    console.error('Failed to get available jobs:', error.message);
  }
}

/**
 * Example: Assign candidate to job
 */
export async function exampleAssignToJob(jobId) {
  try {
    await candidateAPI.assignToJob(jobId);
    console.log(`Successfully assigned to job ${jobId}`);
  } catch (error) {
    console.error('Failed to assign to job:', error.message);
  }
}

/**
 * Example: Get candidate tasks (returns CandidateTaskDto[])
 */
export async function exampleGetTasks() {
  try {
    const tasks = await candidateAPI.getTasks();
    console.log('Candidate tasks:', tasks);
    
    // Each task has CandidateTaskDto structure:
    // { id: number, order: number, description: string, requiresVerification: boolean, 
    //   isVerifiedByHr: boolean, requiresFile: boolean, filePath: string, 
    //   status: Status, completedAt: string | null }
    
    tasks.forEach(task => {
      const statusText = task.status === 0 ? 'NotStarted' : 
                        task.status === 1 ? 'InProgress' : 'Completed';
      console.log(`Task ${task.id}: ${task.description} (${statusText})`);
    });
    
    return tasks;
  } catch (error) {
    console.error('Failed to get tasks:', error.message);
  }
}

/**
 * Example: Complete a task
 */
export async function exampleCompleteTask(taskId) {
  try {
    await candidateAPI.completeTask(taskId);
    console.log(`Task ${taskId} marked as completed`);
  } catch (error) {
    console.error('Failed to complete task:', error.message);
  }
}

/**
 * Example: View candidate profile (returns CandidateProfileDto)
 */
export async function exampleViewProfile() {
  try {
    const profile = await candidateAPI.viewProfile();
    console.log('Candidate profile:', profile);
    
    // Profile has CandidateProfileDto structure:
    // { id: number, email: string, firstName: string, lastName: string, 
    //   resumeUrl: string, assignedJobTitle: string }
    
    console.log(`Profile: ${profile.firstName} ${profile.lastName} (${profile.email})`);
    console.log(`Assigned job: ${profile.assignedJobTitle}`);
    console.log(`Resume: ${profile.resumeUrl}`);
    
    return profile;
  } catch (error) {
    console.error('Failed to view profile:', error.message);
  }
}

/**
 * Example: Edit candidate profile
 */
export async function exampleEditProfile() {
  try {
    // Using CandidateDto DTO
    const profileUpdate = {
      firstName: "John",
      lastName: "Doe"
    };
    
    await candidateAPI.editProfile(profileUpdate);
    console.log('Profile updated successfully');
  } catch (error) {
    console.error('Failed to update profile:', error.message);
  }
}

/**
 * Example: Create a new job (HR only)
 */
export async function exampleCreateJob() {
  try {
    // Using JobDto DTO
    const jobData = {
      title: "Software Developer",
      description: "Full-stack development position with React and .NET"
    };
    
    const response = await hrAPI.createJob(jobData);
    console.log('Job created:', response);
    
    // Response has JobResponse structure:
    // { id: number, title: string, description: string, assignedHrName: string }
    
    return response;
  } catch (error) {
    console.error('Failed to create job:', error.message);
  }
}

/**
 * Example: Create a job task
 */
export async function exampleCreateJobTask(jobId) {
  try {
    // Using JobTaskDto DTO
    const taskData = {
      description: "Complete coding assignment",
      requiresFile: true,
      requiresVerification: true
    };
    
    const response = await hrAPI.createJobTask(jobId, taskData);
    console.log('Job task created:', response);
    
    // Response has TaskResponse structure:
    // { id: number, forJobTitle: string, task: string, requiresFile: boolean,
    //   requiresVerification: boolean, order: number, assignedHrName: string }
    
    return response;
  } catch (error) {
    console.error('Failed to create job task:', error.message);
  }
}

/**
 * Example: Get all candidates (HR only)
 */
export async function exampleGetAllCandidates() {
  try {
    const candidates = await hrAPI.listCandidates();
    console.log('All candidates:', candidates);
    
    // Each candidate has CandidateProfileDto structure
    
    candidates.forEach(candidate => {
      console.log(`Candidate ${candidate.id}: ${candidate.firstName} ${candidate.lastName}`);
      console.log(`  Email: ${candidate.email}`);
      console.log(`  Assigned job: ${candidate.assignedJobTitle}`);
    });
    
    return candidates;
  } catch (error) {
    console.error('Failed to get candidates:', error.message);
  }
}

/**
 * Example: Reorder job tasks
 */
export async function exampleReorderTasks(jobId) {
  try {
    // Using ReorderRequest[] DTO
    const reorderData = [
      { taskId: 1, order: 1 },
      { taskId: 2, order: 2 },
      { taskId: 3, order: 3 }
    ];
    
    await hrAPI.reorderJobTasks(jobId, reorderData);
    console.log('Tasks reordered successfully');
  } catch (error) {
    console.error('Failed to reorder tasks:', error.message);
  }
}

/**
 * Example: Set task requirements
 */
export async function exampleSetTaskRequirements(taskId) {
  try {
    // Set file requirement
    await hrAPI.setTaskFileRequirement(taskId);
    console.log(`Task ${taskId} now requires file upload`);
    
    // Set verification requirement
    await hrAPI.setTaskVerificationRequirement(taskId);
    console.log(`Task ${taskId} now requires HR verification`);
  } catch (error) {
    console.error('Failed to set task requirements:', error.message);
  }
}

/**
 * Example: Verify a completed task
 */
export async function exampleVerifyTask(taskId) {
  try {
    await hrAPI.verifyCompletedTask(taskId);
    console.log(`Task ${taskId} verified by HR`);
  } catch (error) {
    console.error('Failed to verify task:', error.message);
  }
}

/**
 * Example: Upload resume file
 */
export async function exampleUploadResume() {
  try {
    // Create a mock file for demonstration
    const mockFile = new File(['Mock resume content'], 'resume.pdf', { type: 'application/pdf' });
    
    await candidateAPI.uploadResume(mockFile);
    console.log('Resume uploaded successfully');
  } catch (error) {
    console.error('Failed to upload resume:', error.message);
  }
}

// Export all examples for easy access
export const examples = {
  login: exampleLogin,
  candidateRegister: exampleCandidateRegister,
  hrRegister: exampleHrRegister,
  getAvailableJobs: exampleGetAvailableJobs,
  assignToJob: exampleAssignToJob,
  getTasks: exampleGetTasks,
  completeTask: exampleCompleteTask,
  viewProfile: exampleViewProfile,
  editProfile: exampleEditProfile,
  createJob: exampleCreateJob,
  createJobTask: exampleCreateJobTask,
  getAllCandidates: exampleGetAllCandidates,
  reorderTasks: exampleReorderTasks,
  setTaskRequirements: exampleSetTaskRequirements,
  verifyTask: exampleVerifyTask,
  uploadResume: exampleUploadResume
};
