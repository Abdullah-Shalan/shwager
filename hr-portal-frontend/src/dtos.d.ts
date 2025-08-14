/* Auto-generated from backend DTOs (C#) */

export type Status = 0 | 1 | 2 | 'NotStarted' | 'InProgress' | 'Completed';

export interface CandidateDto {
	firstName: string;
	lastName: string;
}

export interface CandidateProfileDto {
	id: number;
	email: string;
	firstName: string;
	lastName: string;
	resumeUrl: string;
	assignedJobTitle: string;
}

export interface CandidateRegisterRequest {
	firstName: string;
	lastName: string;
	email: string;
	password: string;
	resumeUrl: string;
}

export interface CandidateRegisterResponse {
	firstName: string;
	lastName: string;
	email: string;
	resumeUrl: string;
}

export interface CandidateTaskDto {
	id: number;
	order: number;
	description: string;
	requiresVerification: boolean;
	isVerifiedByHr: boolean;
	requiresFile: boolean;
	filePath: string;
	status: Status;
	completedAt: string | null;
}

export interface CandidateTaskProgressDto {
	description: string;
	status: Status;
	documentUrl: string;
	completedAt: string | null;
}

export interface HrRegisterRequest {
	firstName: string;
	lastName: string;
	email: string;
	password: string;
}

export interface HrRegisterResponse {
	firstName: string;
	lastName: string;
	email: string;
}

export interface JobDto {
	title: string;
	description: string;
}

export interface JobResponse {
	id: number;
	title: string;
	description: string;
	assignedHrName: string;
}

export interface JobSummaryDto {
	id: number;
	title: string;
	description: string;
	jobTasks?: JobTaskSummaryDto[] | null;
}

export interface JobTaskDto {
	description: string;
	requiresFile: boolean;
	requiresVerification: boolean;
}

export interface JobTaskSummaryDto {
	id: number;
	taskOrder: number;
	description: string;
	requiresFile: boolean;
	requiresVerification: boolean;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface ReorderRequest {
	taskId: number;
	order: number;
}

export interface TaskResponse {
	id: number;
	forJobTitle: string;
	task: string;
	requiresFile: boolean;
	requiresVerification: boolean;
	order: number;
	assignedHrName: string;
}

export {}; // makes this a module so import() JSDoc works
