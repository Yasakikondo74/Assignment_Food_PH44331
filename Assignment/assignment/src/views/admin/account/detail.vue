<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Account Details</h5>
    </div>
    <div class="card-body">
      <form>
        <div class="mb-3">
          <label class="form-label fw-bold">ID</label>
          <input type="text" class="form-control" :value="editedData.id" disabled />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Username</label>
          <input
            type="text"
            class="form-control"
            v-model="editedData.accUsername"
            @input="markChanged"
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Email</label>
          <input
            type="email"
            class="form-control"
            v-model="editedData.email"
            @input="markChanged"
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Date of Birth</label>
          <input
            type="date"
            class="form-control"
            v-model="editedData.birthDate"
            @change="markChanged"
          />
        </div>
        <div class="mb-3">
          <label class="form-label d-block fw-bold">Role</label>
          <div class="form-check form-check-inline">
            <input
              class="form-check-input"
              type="radio"
              id="roleAdmin"
              value="admin"
              v-model="editedData.accRole"
              @change="markChanged"
            />
            <label class="form-check-label" for="roleAdmin">Admin</label>
          </div>
          <div class="form-check form-check-inline">
            <input
              class="form-check-input"
              type="radio"
              id="roleClient"
              value="client"
              v-model="editedData.accRole"
              @change="markChanged"
            />
            <label class="form-check-label" for="roleClient">Client</label>
          </div>
        </div>
      </form>
    </div>

    <div class="card-footer d-flex justify-content-between">
      <RouterLink to="/admin/account/list" class="btn btn-secondary">Back</RouterLink>

      <div v-if="hasChanges">
        <button class="btn btn-success me-2" @click="saveChanges">Save</button>
        <button class="btn btn-secondary" @click="discardChanges">Discard</button>
      </div>
      <div v-else>
        <button class="btn btn-danger me-2" @click="remove">Delete</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, inject, onMounted, computed } from 'vue';
import { RouterLink, useRouter, useRoute } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const route = useRoute();
const router = useRouter();
const id = route.params.id;
const url = inject('url');

const originalData = ref(null);
const editedData = reactive({
  id: '',
  accUsername: '',
  accPassword: '',
  email: '',
  birthDate: null,
  accRole: 'client',
});

async function callAPI() {
  try {
    const res = await axios.get(url + '/account/' + id);
    const data = res.data;

    // Convert date string to "YYYY-MM-DD" format for date input
    if (data.birthDate) {
      data.birthDate = new Date(data.birthDate).toISOString().split('T')[0];
    }
    
    originalData.value = { ...data };
    Object.assign(editedData, data);
  } catch (err) {
    console.error('Error fetching account data:', err);
  }
}

onMounted(() => {
  callAPI();
});

const hasChanges = computed(() => {
  if (!originalData.value) return false;
  return (
    originalData.value.accUsername !== editedData.accUsername ||
    originalData.value.email !== editedData.email ||
    originalData.value.birthDate !== editedData.birthDate ||
    originalData.value.accRole !== editedData.accRole
  );
});

function discardChanges() {
  if (originalData.value) {
    Object.assign(editedData, originalData.value);
  }
}

async function saveChanges() {
  try {
    const payload = { ...editedData };
    await axios.put(url + '/account/' + id, payload);
    alert('Account updated successfully');
    await callAPI(); // Refresh the data to reflect the latest changes
  } catch (err) {
    console.error('Failed to save changes:', err);
    alert('Failed to save changes. Check the console for more details.');
  }
}

async function remove() {
  if (confirm('Are you sure you want to delete this account?')) {
    try {
      await axios.delete(url + '/account/' + id);
      alert('Account deleted successfully');
      router.push('/admin/account/list');
    } catch (err) {
      console.error('Failed to delete account:', err);
      alert('Failed to delete account.');
    }
  }
}
</script>

<style lang="sass" scoped>
span
  font-size: 150%
</style>