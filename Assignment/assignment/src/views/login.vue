<template>
  <div class="login-container">
    <div class="wrapper">
      <form @submit.prevent="handleLogin">
        <h2>Login</h2>

        <div class="input-field">
          <input type="text" v-model="username" required />
          <label>Enter your username</label>
        </div>

        <div class="input-field">
          <input type="password" v-model="password" required />
          <label>Enter your password</label>
        </div>

        <div class="forget">
          <label for="remember">
            <input type="checkbox" id="remember" />
            <p>Remember me</p>
          </label>
          <a href="#">Forgot password?</a>
        </div>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Logging in...' : 'Log In' }}
        </button>

        <div v-if="errorMessage" style="color: #ffb3b3; margin-top: 10px">
          {{ errorMessage }}
        </div>

        <div class="register">
          <p>
            Don't have an account?
            <router-link to="/signup">Register</router-link>
          </p>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { inject, ref } from 'vue'
import { useRouter } from 'vue-router'

const url = inject("url")
const router = useRouter()

const username = ref('')
const password = ref('')
const errorMessage = ref('')
const loading = ref(false)
const emitter = inject('emitter')

async function handleLogin() {
  errorMessage.value = ''
  loading.value = true

  try {
    const res = await fetch(`${url}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({
        username: username.value,
        password: password.value
      })
    })

    if (!res.ok) {
      if (res.status === 401) {
        errorMessage.value = "Invalid username or password."
      } else {
        const err = await res.json()
        errorMessage.value = err.message || "Login failed."
      }
      return
    }

    const result = await res.json()
    localStorage.setItem('userId', result.id)
    localStorage.setItem("username", username.value)
    localStorage.setItem("role", result.role)
    emitter.emit('login-success')
    router.push("/")

  } catch (err) {
    errorMessage.value = "Network or server error."
    console.error(err)
  } finally {
    loading.value = false
  }
}
</script>
<style lang="sass">
@import url("https://fonts.googleapis.com/css2?family=Open+Sans:wght@200;300;400;500;600;700&display=swap")

* 
  margin: 0
  padding: 0
  box-sizing: border-box
  font-family: "Open Sans", sans-serif

.login-container
  display: flex
  align-items: center
  justify-content: center
  min-height: 100vh
  width: 100%
  padding: 0 10px
  position: relative

  &::before
    content: ""
    position: absolute
    width: 100%
    height: 100%
    background: url("../assets/login-hero-bg.jpg"), #000
    background-position: center
    background-size: cover
    z-index: -1

.wrapper
  width: 400px
  border-radius: 8px
  padding: 30px
  text-align: center
  border: 1px solid rgba(255, 255, 255, 0.5)
  backdrop-filter: blur(8px)
  -webkit-backdrop-filter: blur(8px)

form
  display: flex
  flex-direction: column

  h2
    font-size: 2rem
    margin-bottom: 20px
    color: #fff

  .input-field
    position: relative
    border-bottom: 2px solid #ccc
    margin: 15px 0

    label
      position: absolute
      top: 50%
      left: 0
      transform: translateY(-50%)
      color: #fff
      font-size: 16px
      pointer-events: none
      transition: 0.15s ease

    input
      width: 100%
      height: 40px
      background: transparent
      border: none
      outline: none
      font-size: 16px
      color: #fff

      &:focus ~ label,
      &:valid ~ label
        font-size: 0.8rem
        top: 10px
        transform: translateY(-120%)

  .forget
    display: flex
    align-items: center
    justify-content: space-between
    margin: 25px 0 35px 0
    color: #fff

    label
      display: flex
      align-items: center

      p
        margin-left: 8px

    #remember
      accent-color: #fff

    a
      color: inherit
      text-decoration: none

      &:hover
        text-decoration: underline

  button
    background: #fff
    color: #000
    font-weight: 600
    border: none
    padding: 12px 20px
    cursor: pointer
    border-radius: 3px
    font-size: 16px
    border: 2px solid transparent
    transition: 0.3s ease

    &:hover
      color: #fff
      border-color: #fff
      background: rgba(255, 255, 255, 0.15)

  .register
    text-align: center
    margin-top: 30px
    color: #fff

    a
      color: inherit
      text-decoration: none

      &:hover
        text-decoration: underline
</style>