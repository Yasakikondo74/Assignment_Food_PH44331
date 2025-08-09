// authStore.js
import { inject, reactive, watch } from 'vue'

export const authStore = reactive({
  user: null,
  isLoggedIn: false,
  isAdmin: false
})

let apiUrl
export function initAuthStore(providedUrl) {
  apiUrl = providedUrl
}

export async function fetchUser() {
  const username = localStorage.getItem('username')
  const role = localStorage.getItem('role')

  if (!username || !role) {
    logout()
    return
  }

  try {
    const response = await fetch(`${apiUrl}/account/${username}`, {
      method: 'GET',
      credentials: 'include',
    })
    if (response.ok) {
      const data = await response.json()
      authStore.user = { ...data, accRole: role }
      authStore.isLoggedIn = true
      authStore.isAdmin = authStore.user.accRole?.toLowerCase() === 'admin'
    } else {
      logout()
    }
  } catch (err) {
    console.error('Error fetching user:', err)
    logout()
  }
}

export function logout() {
  localStorage.removeItem('username')
  localStorage.removeItem('role')
  authStore.user = null
  authStore.isLoggedIn = false
  authStore.isAdmin = false
}
