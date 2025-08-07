<template>
  <table class="table table-striped table-hover">
    <thead>
      <tr>
        <th scope="col">ID</th>
        <th scope="col">Account ID</th>
        <th scope="col">Created At</th>
        <th scope="col">Functions</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="receipt in receipt_data" :key="receipt.id">
        <td class="fw-bold">{{ receipt.id }}</td>
        <td>{{ receipt.accountId }}</td>
        <td>{{ new Date(receipt.createdAt).toLocaleString() }}</td>
        <td>
          <RouterLink :to="`/admin/receipt/detail/${receipt.id}`" class="btn btn-secondary btn-sm">
            View Details
          </RouterLink>
        </td>
      </tr>
    </tbody>
  </table>
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
  }
}

onMounted(() => {
  callAPI();
});
</script>

<style lang="sass" scoped>
span
  font-size: 150%
</style>