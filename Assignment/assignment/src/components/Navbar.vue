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
            <router-link class="nav-link" to="/client/food/list">Food List</router-link>
          </li>
          <li v-if="isCustomer">
            <router-link class="nav-link" to="/client/food/buy">Buy</router-link>
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
              <i class="bi bi-person-circle me-1"></i> {{ user.accUsername }}
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
              <li v-if="isAdmin">
                <router-link class="dropdown-item" to="/admin/food/list">Food List</router-link>
              </li>
              <li v-if="isAdmin">
                <router-link class="dropdown-item" to="/admin/account/list">Account List</router-link>
              </li>
              <li v-if="isAdmin">
                <a class="dropdown-item" href="/admin/receipt/list">Receipt List</a>
              </li>
              <li v-if="isAdmin">
                <a class="dropdown-item" href="/admin/receipt_detail/list">Receipt Detail List</a>
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
import { inject, ref, onMounted, watchEffect, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const url = inject('url')
const router = useRouter()
const route = useRoute()

const user = ref(null)
const isDarkMode = ref(false)

const isAdmin = computed(() => { return user.value?.accRole?.toLowerCase() == 'admin     ' })
const isCustomer = computed(() => { return user.value?.accRole?.toLowerCase() == 'customer  ' })

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
    console.log("No user found in localStorage. Setting user to null.")
    user.value = null
    return
  }

  try {
    const response = await fetch(`${url}/account/${username}`, {
      method: 'GET',
      credentials: 'include',
    })

    if (!response.ok) {
      const errorText = await response.text()
      console.error(`User fetch failed: ${response.status} - ${response.statusText}`, errorText)
      logout() 
      return
    }

    const data = await response.json()
    user.value = { ...data, accRole: role }
    console.log("Successfully fetched user data:", user.value)

  } catch (err) {
    console.error('Error in updateUser:', err)
    user.value = { username, accRole: role }
    console.log("Using fallback user data:", user.value)
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
})

// Watch the route object for changes. When a user logs in and navigates,
// the route changes, which will trigger this watch function.
watch(() => route.path, () => {
  updateUser()
})

// Initial call on mount
onMounted(() => {
  isDarkMode.value = localStorage.getItem('darkMode') === 'true'
  updateUser()
})

</script>