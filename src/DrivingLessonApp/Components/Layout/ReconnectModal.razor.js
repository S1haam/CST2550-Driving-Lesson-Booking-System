// Set up event handlers

//Refernce reconnect modal element
const reconnectModal = document.getElementById("components-reconnect-modal");

//Listen for Blazor reconnect state changes such as show, hide, failed, rejected
reconnectModal.addEventListener("components-reconnect-state-changed", handleReconnectStateChanged);

//Retry button for reconnection failure
const retryButton = document.getElementById("components-reconnect-button");
retryButton.addEventListener("click", retry);

//Resume button for server pausing session
const resumeButton = document.getElementById("components-resume-button");
resumeButton.addEventListener("click", resume);

//Function to handle reconnect state changes
function handleReconnectStateChanged(event) {

    //Show reconnect modal
    if (event.detail.state === "show") {
        reconnectModal.showModal();
    } else if (event.detail.state === "hide") {

        //Hide modal
        reconnectModal.close();
    } else if (event.detail.state === "failed") {

        //Failed reconnect attempt and retry button is displayed
        document.addEventListener("visibilitychange", retryWhenDocumentBecomesVisible);

        //Server rejection and reloads page
    } else if (event.detail.state === "rejected") {
        location.reload();
    }
}

//Function retrying to reconnect to server
async function retry() {
    
    //Stop Listening for changes during retry
    document.removeEventListener("visibilitychange", retryWhenDocumentBecomesVisible);

    try {
        // Reconnect will asynchronously return:
        // - true to mean success
        // - false to mean we reached the server, but it rejected the connection (e.g., unknown circuit ID)
        // - exception to mean we didn't reach the server (this can be sync or async)
        const successful = await Blazor.reconnect();
        if (!successful) {
            // We have been able to reach the server, but the circuit is no longer available.
            // We'll reload the page so the user can continue using the app as quickly as possible.
            const resumeSuccessful = await Blazor.resumeCircuit();
            if (!resumeSuccessful) {
                location.reload();
            } else {
                reconnectModal.close();
            }
        }
    } catch (err) {
        // We got an exception, server is currently unavailable
        document.addEventListener("visibilitychange", retryWhenDocumentBecomesVisible);
    }
}

//Function to resume after server paused session
async function resume() {

    //If resume fails, reload page
    try {
        const successful = await Blazor.resumeCircuit();
        if (!successful) {
            location.reload();
        }
    } catch {
        //Update UI to display failure state
        reconnectModal.classList.replace("components-reconnect-paused", "components-reconnect-resume-failed");
    }
}

//Function to retry automatically when user returns to the tab
async function retryWhenDocumentBecomesVisible() {
    
    //Retry only when the tab becomes visible
    if (document.visibilityState === "visible") {
        await retry();
    }
}
