<template>
    <div class="not-found">
        <h1>404 - Not Found</h1>
        <p>Non existent website.</p>
        <RouterLink to="/">Go back to Main Menu</RouterLink>
    </div>
</template>

<script setup>
import { RouterLink } from 'vue-router';
</script>

<style scoped>
.not-found {
    text-align: center;
    margin-top: 100px;
}
</style>