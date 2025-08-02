<template>
    <nav class="navbar navbar-expand-lg bg-body-tertiary">
        <div class="container-fluid">
            <a class="navbar-brand" href="#">Navbar</a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" 
                aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <router-link class="nav-link" aria-current="page" :to="{ name: 'home' }">Home</router-link>
                    </li>
                    <li>                                 
                        <router-link class="nav-link" to="/client/food">
                            Food List
                        </router-link>
                    </li>
                    <li class="nav-item">
                        <div class="dropdown">
                            <button class="btn dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                Admin Features
                            </button>
                            <ul class="dropdown-menu">
                                <li>
                                    <router-link class="dropdown-item" to="/admin/food">Food List</router-link>
                                </li>
                                <li><a class="dropdown-item" href="#">Account List</a></li>
                                <li><a class="dropdown-item" href="#">Order List</a></li>
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</template>

<script setup>
import { RouterLink } from 'vue-router'
let isAdmin = false
</script>