# Video Manager

A lightweight web application for managing and playing MP4 video files, built with ASP.NET Core and modern JavaScript.

## Features

- Single-page application with tab-based navigation
- Upload MP4 files with instant validation
- Browse uploaded videos in a responsive table
- Built-in video player with native HTML5 controls

## Technical Stack

- **Backend**: ASP.NET Core 9.0
- **Frontend**: Vanilla JavaScript, Bootstrap 5
- **Storage**: Local filesystem (development setup)
- **Video Format**: MP4 only
- **Max Upload Size**: 200MB per file

## API Endpoints

- `GET /api/videos` - List all uploaded videos
- `POST /api/videos` - Upload a single video file
- `DELETE /api/videos/{filename}` - Remove a video

## Limitations (Minimum Build)

This is a minimal implementation intended for development purposes. For production use, several enhancements would be necessary:

### Storage
- Replace local filesystem storage with cloud storage (e.g., Azure Blob Storage)
- Add video transcoding for different formats/qualities

### Security
- Add authentication/authorization
- Implement CSRF protection
- Add request validation middleware
- Implement proper CORS policies
- Add rate limiting for API endpoints

### Testing
- Unit tests for controllers and services
- Integration tests for API endpoints
- E2E tests for user workflows
- Performance testing for file uploads
- Browser compatibility testing

## Getting Started

1. Clone the repository
2. Ensure .NET 8.0 SDK is installed
3. Run with IIS Express in Visual Studio


## Development

The project uses standard ASP.NET Core conventions:
- Controllers handle HTTP requests
- Static files served from wwwroot
- Bootstrap for basic styling
- Vanilla JavaScript for interactivity