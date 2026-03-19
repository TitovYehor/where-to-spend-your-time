# Frontend Application

React + TypeScript frontend built with Vite and TailwindCSS.
The application communicates with a **.NET Web API** backend to handle authentication and application data.

---

# Tech Stack

* React
* TypeScript
* Vite
* TailwindCSS
* Axios

---

# Getting Started

## Install dependencies

npm install

## Run development server

npm run dev

The application will start on:

http://localhost:5173

---

# Environment Variables

This project uses environment variables for configuration.

## Local Development

Create a `.env.local` file in the project root:

VITE_API_URL=https://localhost:7005

This variable defines the base URL of the backend API.

Example request:

https://localhost:7005/api/auth/login

`.env.local` should **not be committed** to the repository.

---

## Production

In production the `VITE_API_URL` variable is provided by the hosting platform.

Example:

VITE_API_URL=https://where-to-spend-your-time-backend.onrender.com

---

# Project Structure

src/
├── api/            # Axios configuration and API endpoints
├── services/       # Backend communication layer
├── components/     # Reusable UI components
├── pages/          # Page-level components
├── contexts/       # React Context providers (authentication, global state)
├── hooks/          # Custom React hooks
├── types/          # TypeScript interfaces and models
├── utils/          # Utility functions

---

# API Communication

All communication with the backend API is handled through **service modules**.

Example service:

services/authService.ts

Components should call **services**, not Axios directly.

---

# Axios Configuration

Axios is configured in:

src/api/axios.ts

The configuration uses the `VITE_API_URL` environment variable as the API base URL.

---

# Authentication

Authentication is handled by the backend API.

Typical login flow:

1. User submits login form
2. Frontend calls `authService.login`
3. Backend validates credentials
4. Backend sets authentication cookie
5. Axios sends the cookie automatically in future requests

---

# Build for Production

Create a production build:

npm run build

Preview the production build locally:

npm run preview

---

# Linting

Run ESLint:

npm run lint

---

# Notes

* API requests should always be implemented through the `services` layer.
* Components should focus on UI logic and avoid direct API calls.
* TypeScript types are stored in the `types` directory.