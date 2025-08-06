import { reactive, watch } from 'vue'

export const authStore = reactive({
  user: null,
  isLoggedIn: false,
  isAdmin: false
})

export async function fetchUser() {
  const username = localStorage.getItem('username')
  const role = localStorage.getItem('role')

  if (!username || !role) {
    authStore.user = null
    authStore.isLoggedIn = false
    authStore.isAdmin = false
    return
  }
  const url = 'http://localhost:7295' 
  try {
    const response = await fetch(`${url}/account/${username}`, {
      method: 'GET',
      credentials: 'include',
    })

    if (response.ok) {
      const data = await response.json()
      authStore.user = { ...data, accRole: role }
      authStore.isLoggedIn = true
      authStore.isAdmin = authStore.user.accRole?.toLowerCase() === 'admin'
    } else {
      console.error('Failed to fetch user:', response.status, response.statusText)
      logout() // Log out on error
    }
  } catch (err) {
    console.error('Error fetching user:', err)
    logout() // Log out on error
  }
}

export function logout() {
  localStorage.removeItem('username')
  localStorage.removeItem('role')
  authStore.user = null
  authStore.isLoggedIn = false
  authStore.isAdmin = false
}