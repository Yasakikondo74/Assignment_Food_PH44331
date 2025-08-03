<template>
  <nav :class="['navbar navbar-expand-lg', isDarkMode ? 'bg-dark navbar-dark' : 'bg-body-tertiary']">
    <div class="container-fluid">
      <a class="navbar-brand" href="#">Navbar</a>

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav me-auto">
          <li class="nav-item">
            <router-link class="nav-link" :to="{ name: 'home' }">Home</router-link>
          </li>
          <li>
            <router-link class="nav-link" to="/client/food">Food List</router-link>
          </li>
        </ul>

        <div class="d-flex align-items-center">
          <div v-if="user" class="dropdown">
            <button
              :class="['btn dropdown-toggle d-flex align-items-center', isDarkMode ? 'btn-secondary' : 'btn-outline-secondary']"
              type="button"
              id="userMenu"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              <i class="bi bi-person-circle me-1"></i> {{ user.username }}
            </button>
            <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="userMenu">
              <li>
                <a class="dropdown-item" href="#">Account Details</a>
              </li>
              <li>
                <button class="dropdown-item" @click="toggleTheme">
                  {{ isDarkMode ? 'Light Mode' : 'Dark Mode' }}
                </button>
              </li>
              <li v-if="user.accRole === 'admin'">
                <router-link class="dropdown-item" to="/admin/food">Food List</router-link>
              </li>
              <li v-if="user.accRole === 'admin'">
                <a class="dropdown-item" href="#">Account List</a>
              </li>
              <li v-if="user.accRole === 'admin'">
                <a class="dropdown-item" href="#">Order List</a>
              </li>
              <li>
                <button class="dropdown-item text-danger" @click="logout">Logout</button>
              </li>
            </ul>
          </div>
          <router-link v-else class="btn btn-outline-primary" to="/login">Login</router-link>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { inject, ref, onMounted, onBeforeUnmount, watchEffect } from 'vue'
import { useRouter } from 'vue-router'

const url = inject('url')
const emitter = inject('emitter')
const router = useRouter()

const user = ref(null)
const isDarkMode = ref(false)

function applyTheme() {
  document.documentElement.setAttribute('data-bs-theme', isDarkMode.value ? 'dark' : 'light')
}

function toggleTheme() {
  isDarkMode.value = !isDarkMode.value
  localStorage.setItem('darkMode', isDarkMode.value)
}

async function updateUser() {
  const username = localStorage.getItem('username')
  const role = localStorage.getItem('role')
  if (!username || !role) {
    user.value = null
    return
  }

  try {
    const response = await fetch(`${url}/account/${username}`)
    if (!response.ok) throw new Error('User fetch failed')
    const data = await response.json()
    user.value = data
  } catch (err) {
    console.error(err)
    user.value = { username, acc_role: role } 
  }
}

function logout() {
  localStorage.removeItem('username')
  localStorage.removeItem('role')
  user.value = null
  router.push('/login')
}

watchEffect(() => {
  applyTheme()
});

onMounted(() => {
  isDarkMode.value = localStorage.getItem('darkMode') === 'true'
  updateUser()
  emitter.on('login-success', updateUser)
})

onBeforeUnmount(() => {
  emitter.off('login-success', updateUser)
})
console.log(user.value?.acc_role)
</script>