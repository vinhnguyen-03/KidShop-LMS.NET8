// Chatbot.js
let chatHistory = [];
let isWaitingForReply = false; // để tránh spam khi AI chưa trả lời

// Hiển thị tin nhắn
function addMessage(sender, text) {
    const box = document.getElementById("chatMessages");
    const msg = document.createElement("div");
    msg.className = `message ${sender}`;
    msg.textContent = text;
    box.appendChild(msg);
    box.scrollTop = box.scrollHeight;

    chatHistory.push({ sender, text });
}

// Gửi tin nhắn đến AI chuyên gia
async function sendExpertMessage() {
    if (isWaitingForReply) return; // nếu AI đang trả lời, không gửi thêm
    const input = document.getElementById("userInput");
    const text = input.value.trim();
    if (!text) return;

    addMessage("user", text);
    input.value = "";
    addMessage("ai", "Chuyên gia AI đang trả lời...");

    // Lấy 5 tin nhắn gần nhất để gửi
    const recentHistory = chatHistory.slice(-5);
    const contextText = recentHistory.map(m => `${m.sender}: ${m.text}`).join("\n");

    isWaitingForReply = true;

    try {
        const res = await fetch("/api/chatbot/chat", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                prompt: text,
                context: contextText
            })
        });

        if (res.status === 429) {
            const msgContainer = document.querySelector(".message.ai:last-child");
            if (msgContainer) msgContainer.remove();
            addMessage("ai", "AI đang bận, vui lòng thử lại sau vài giây.");
            isWaitingForReply = false;
            return;
        }

        const data = await res.json();
        const msgContainer = document.querySelector(".message.ai:last-child");
        if (msgContainer) msgContainer.remove();
        addMessage("ai", data.reply || "Chuyên gia AI chưa thể trả lời.");
    } catch (err) {
        console.error(err);
        const msgContainer = document.querySelector(".message.ai:last-child");
        if (msgContainer) msgContainer.remove();
        addMessage("ai", "Lỗi kết nối server.");
    } finally {
        isWaitingForReply = false;
    }
}

// Khởi tạo sự kiện mở/đóng chatbox và gửi tin nhắn
function initExpertChat() {
    const btnChat = document.getElementById("btnChatAI");
    const btnClose = document.getElementById("btnCloseChat");
    const btnSend = document.getElementById("btnSend");
    const input = document.getElementById("userInput");

    btnChat.onclick = () => document.getElementById("chatBoxAI").classList.toggle("hidden");
    btnClose.onclick = () => document.getElementById("chatBoxAI").classList.add("hidden");

    btnSend.onclick = sendExpertMessage;
    input.addEventListener("keypress", function (e) {
        if (e.key === "Enter") sendExpertMessage();
    });
}

// Khởi tạo khi script load
document.addEventListener("DOMContentLoaded", initExpertChat);