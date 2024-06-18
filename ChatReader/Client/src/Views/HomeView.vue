<script lang="ts">
const nick = import.meta.env.VITE_NICK;
const userId = import.meta.env.VITE_USER_ID;
const userSecret = import.meta.env.VITE_USER_SECRET;

export default {
    methods: {
        async auth() {
            const url = "http://localhost:5293/auth/";

            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ nick, userId, userSecret }),
            });

            if (response.ok) {
                window.location.href = `https://id.twitch.tv/oauth2/authorize?response_type=code&client_id=${userId}&redirect_uri=${url}&scope=chat:read`
            }
        }
    }
}
</script>

<template>
    <h1 class="header">Authenticate with Twitch</h1>
    <button class="button" @click="auth">
        <span>Connect with Twitch</span>
    </button>
</template>

<style scoped>
.header {
    color: #fffefe;
}

.button {
    background-color: #9046ff;
    color: #fffefe;
    padding: 6px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}

.button:hover {
    background-color: #772de9
}
</style>
