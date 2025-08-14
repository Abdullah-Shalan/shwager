# Shwager - شواغر

Shwager is a Software as a Service (SaaS) solution designed to streamline the job application process. It offers a comprehensive platform for both job seekers and hiring teams, simplifying the application workflow and enhancing recruitment efficiency.

## Features

* **Centralized Dashboard**: Access and manage all your job applications in one place.
* **Automated Application Tracking**: Receive real-time updates on application statuses.
* **Customizable Job Listings**: Tailor job postings to attract the right candidates.
* **Integrated Communication Tools**: Facilitate seamless communication between applicants and recruiters.
* **Analytics & Reporting**: Gain insights into application trends and recruitment metrics.

## Technologies Used

* **Backend**: C#
* **Frontend**: JavaScript, HTML

## Installation

To set up Shwager locally, follow these steps:

1. Clone the repository:

   ```bash
   git clone https://github.com/Abdullah-Shalan/shwager.git
   ```

2. Navigate to the project directory:

   ```bash
   cd shwager
   ```

3. Follow the setup instructions for the backend and frontend components as specified in their respective directories.

## Database Setup

Shwager uses a SQL database to store all job and candidate information.

1. Acquire your database connection string from your SQL server instance (e.g., `Server=YOUR_SERVER;Database=ShwagerDB;User Id=USERNAME;Password=PASSWORD;`).

2. Open the `appsettings.json` file in the backend directory.

3. Replace the default connection string under `ConnectionStrings` with your own:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ShwagerDB;User Id=USERNAME;Password=PASSWORD;"
     }
   }
   ```

4. Ensure the database exists and the credentials have proper permissions.

5. Run migrations to create the necessary tables:

   ```bash
   dotnet ef database update
   ```

## Usage

After setting up the backend, frontend, and database, open your browser and navigate to `http://localhost:3000` to access Shwager.