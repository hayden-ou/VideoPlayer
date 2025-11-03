// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {    
    const tabUpload = document.getElementById("tab-upload");
    const tabCatalogue = document.getElementById("tab-catalogue");
    const uploadView = document.getElementById("upload-view");
    const catalogueView = document.getElementById("catalogue-view");
    const uploadBtn = document.getElementById("uploadBtn");
    const uploadStatus = document.getElementById("uploadStatus");
    const inputFiles = document.getElementById("videoFiles");
    const catalogueTableBody = document.querySelector("#catalogueTable tbody");
    const videoPlayer = document.getElementById("videoPlayer");

    function showUpload() {
        tabUpload.classList.add("active");
        tabUpload.setAttribute("aria-selected", "true");
        tabCatalogue.classList.remove("active");
        tabCatalogue.setAttribute("aria-selected", "false");
        uploadView.classList.remove("hidden");
        catalogueView.classList.add("hidden");
        uploadStatus.textContent = "";
    }

    function showCatalogue() {
        tabCatalogue.classList.add("active");
        tabCatalogue.setAttribute("aria-selected", "true");
        tabUpload.classList.remove("active");
        tabUpload.setAttribute("aria-selected", "false");
        catalogueView.classList.remove("hidden");
        uploadView.classList.add("hidden");
        loadCatalogue();
    }

    tabUpload.addEventListener("click", showUpload);
    tabCatalogue.addEventListener("click", showCatalogue);

    uploadBtn.addEventListener("click", async () => {
        const file = inputFiles.files[0];
        if (!file) {
            uploadStatus.textContent = "Please select an MP4 file to upload.";
            return;
        }

        if (!file.name.toLowerCase().endsWith(".mp4")) {
            uploadStatus.textContent = "Only MP4 files are allowed.";
            return;
        }

        uploadStatus.textContent = "Uploading...";
        uploadBtn.disabled = true;

        try {
            const form = new FormData();
            form.append("videoFile", file);

            const resp = await fetch("/api/videos", {
                method: "POST",
                body: form
            });

            const responseData = await resp.json();
            
            if (!resp.ok) {
                uploadStatus.textContent = responseData.message || "Upload failed. Please try again.";
                return;
            }

            uploadStatus.textContent = "Upload successful.";
            inputFiles.value = "";
            showCatalogue();
        } catch (err) {
            uploadStatus.textContent = "An error occurred. Please try again.";
        } finally {
            uploadBtn.disabled = false;
        }
    });

    async function loadCatalogue() {
        catalogueTableBody.innerHTML = "<tr><td colspan='2'>Loading…</td></tr>";
        
        try {
            const resp = await fetch("/api/videos");
            if (!resp.ok) {
                catalogueTableBody.innerHTML = "<tr><td colspan='2'>Failed to load catalogue. Please try again.</td></tr>";
                return;
            }

            const files = await resp.json();
            if (!Array.isArray(files) || files.length === 0) {
                catalogueTableBody.innerHTML = "<tr><td colspan='2'>No videos found</td></tr>";
                return;
            }

            catalogueTableBody.innerHTML = "";
            files.forEach(f => appendVideoRow(f));
        } catch {
            catalogueTableBody.innerHTML = "<tr><td colspan='2'>Error loading catalogue</td></tr>";
        }
    }

    function appendVideoRow(fileData) {
        const tr = document.createElement("tr");
        tr.tabIndex = 0;
        
        const nameTd = document.createElement("td");
        nameTd.textContent = fileData.fileName;
        
        const sizeTd = document.createElement("td");
        sizeTd.textContent = formatBytes(fileData.size || 0);
        
        tr.append(nameTd, sizeTd);

        tr.addEventListener("click", () => {
            catalogueTableBody.querySelectorAll("tr").forEach(r => r.classList.remove("active"));
            tr.classList.add("active");
            playVideo(fileData.fileName);
        });

        tr.addEventListener("keypress", (e) => {
            if (e.key === "Enter" || e.key === " ") {
                tr.click();
                e.preventDefault();
            }
        });

        catalogueTableBody.appendChild(tr);
    }

    function playVideo(fileName) {
        if (!fileName) return;
        videoPlayer.pause();
        videoPlayer.src = `/videos/${encodeURIComponent(fileName)}`;
        videoPlayer.load();
        videoPlayer.play().catch(() => {});
        videoPlayer.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    function formatBytes(bytes, decimals = 1) {
        if (bytes === 0) return "0 B";
        const k = 1024;
        const dm = decimals < 0 ? 0 : decimals;
        const sizes = ["B", "KB", "MB", "GB", "TB"];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`;
    }

    // Initialize in Upload view
    showUpload();
});
