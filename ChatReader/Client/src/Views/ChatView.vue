<script lang="ts">
export default {
    data() {
        return {
            connection: {} as WebSocket
        }
    },
    created() {
        this.connection = new WebSocket("ws://localhost:5293/ws");

        this.connection.onopen = () => {
            console.log("Connection opened");
        }

        this.connection.onmessage = (event) => {
            console.log(event);
        }

        this.connection.onerror = (event) => {
            console.log(event);
        }

        this.connection.onclose = () => {
            console.log("Connection closed");
        }
    }
}; 
</script>

<template>
    <h1 class="header">Live chat</h1>
</template>

<style scoped>
.header {
    color: #fffefe;
}
</style>
