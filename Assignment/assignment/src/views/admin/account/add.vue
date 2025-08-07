<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Add New Account</h5>
    </div>
    <div class="card-body">
      <form @submit.prevent="addAccount">
        <div class="mb-3">
          <label class="form-label fw-bold">Username</label>
          <input
            type="text"
            class="form-control"
            v-model="account.accUsername"
            required
            placeholder="e.g., john.doe"
          />
        </div>
        <div class="mb-3">
        <label class="form-label fw-bold">Password</label>
          <input
            type="password"
            class="form-control"
            v-model="account.accPassword"
            required
            placeholder="Choose a strong password"
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Email</label>
          <input
            type="email"
            class="form-control"
            v-model="account.email"
            required
            placeholder="e.g., john.doe@example.com"
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Date of Birth</label>
          <input type="date" class="form-control" v-model="account.birthDate" />
        </div>
        <div class="mb-3">
          <label class="form-label d-block fw-bold">Role</label>
          <div class="form-check form-check-inline">
            <input
              class="form-check-input"
              type="radio"
              id="roleAdmin"
              value="admin"
              v-model="account.accRole"
            />
            <label class="form-check-label" for="roleAdmin">Admin</label>
          </div>
          <div class="form-check form-check-inline">
            <input
              class="form-check-input"
              type="radio"
              id="roleClient"
              value="client"
              v-model="account.accRole"
            />
            <label class="form-check-label" for="roleClient">Client</label>
          </div>
        </div>
        <button class="btn btn-success" type="submit">Add Account</button>
        <RouterLink to="/admin/account/list" class="btn btn-secondary ms-2">Back</RouterLink>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, inject } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const router = useRouter();
const url = inject('url');

const account = reactive({
  accUsername: '',
  accPassword: '',
  email: '',
  birthDate: null,
  accRole: 'client', // default to client role
});

async function addAccount() {
  try {
    await axios.post(url + '/account', account);
    alert('Account added successfully');
    router.push('/admin/account/list');
  } catch (err) {
    console.error('Failed to add account:', err);
    alert('Failed to add account. Check the console for more details.');
  }
}
</script>

<style lang="sass" scoped>
span
  font-size: 150%
</style>