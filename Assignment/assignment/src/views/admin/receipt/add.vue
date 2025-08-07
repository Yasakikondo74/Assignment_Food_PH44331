<template>
  <div class="container-fluid">
    <div class="row">
      <div class="col-12">
        <h3 class="mt-4 mb-3">Receipt List (Admin)</h3>
        <div class="card shadow-sm">
          <div class="card-body">
            <table class="table table-striped table-hover">
              <thead>
                <tr>
                  <th scope="col">Receipt ID</th>
                  <th scope="col">Account ID</th>
                  <th scope="col">Created At</th>
                  <th scope="col">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="x in receipt_data" :key="x.id">
                  <td class="fw-bold">{{ x.id }}</td>
                  <td>{{ x.accountId }}</td>
                  <td>{{ formatDate(x.createdAt) }}</td>
                  <td>
                    <button class="btn btn-danger btn-sm me-1" @click="remove(x.id)">
                      Delete
                    </button>
                    <RouterLink
                      :to="`/admin/receipt/detail/${x.id}`"
                      class="btn btn-info btn-sm text-white"
                    >
                      Show Details
                    </RouterLink>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, onMounted } from 'vue';
import { RouterLink } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const url = inject('url');
const receipt_data = ref([]);

async function callAPI() {
  try {
    const res = await axios.get(url + '/receipt');
    receipt_data.value = res.data;
  } catch (error) {
    console.error('Failed to fetch receipt data:', error);
    alert('Failed to fetch receipt data.');
  }
}

function formatDate(dateString) {
  if (!dateString) return 'N/A';
  const options = { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' };
  return new Date(dateString).toLocaleDateString(undefined, options);
}

async function remove(id) {
  if (confirm('Are you sure you want to delete this receipt? This action cannot be undone.')) {
    try {
      await axios.delete(`${url}/receipt/${id}`);
      alert('Successfully Deleted');
      await callAPI(); // Refresh the data after deletion
    } catch (error) {
      console.error('Failed to delete receipt:', error);
      alert('Failed to delete receipt.');
    }
  }
}

onMounted(() => {
  callAPI();
});
</script>