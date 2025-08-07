<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title fw-bolder">Add Item to Receipt</h5>
    </div>
    <div class="card-body">
      <form @submit.prevent="addReceiptDetail">
        <div class="mb-3">
          <label class="form-label fw-bold">Receipt ID</label>
          <input
            type="text"
            class="form-control"
            v-model="receiptDetail.receiptId"
            disabled
            placeholder="Receipt ID is pre-filled from URL"
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Food ID</label>
          <input
            type="text"
            class="form-control"
            v-model="receiptDetail.foodId"
            required
            placeholder="Enter a valid Food ID"
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Quantity</label>
          <input
            type="number"
            class="form-control"
            v-model.number="receiptDetail.quantity"
            min="1"
            required
          />
        </div>
        <div class="mb-3">
          <label class="form-label fw-bold">Total Cost</label>
          <input
            type="number"
            class="form-control"
            v-model.number="receiptDetail.totalCost"
            min="0"
            step="0.01"
            required
          />
        </div>

        <button class="btn btn-success" type="submit">Add Item</button>
        <RouterLink :to="`/admin/receipt/detail/${receiptDetail.receiptId}`" class="btn btn-secondary ms-2">
          Back to Receipt
        </RouterLink>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, inject, onMounted } from 'vue';
import { RouterLink, useRouter, useRoute } from 'vue-router';
import axios from 'axios';

axios.defaults.withCredentials = true;

const router = useRouter();
const route = useRoute();
const url = inject('url');

const receiptDetail = reactive({
  receiptId: '',
  foodId: '',
  quantity: 1,
  totalCost: 0,
});

// Pre-fill the receiptId from the URL parameter
onMounted(() => {
  receiptDetail.receiptId = route.params.receiptId;
});

async function addReceiptDetail() {
  try {
    await axios.post(url + '/receipt-detail', receiptDetail);
    alert('Receipt item added successfully');
    router.push(`/admin/receipt/detail/${receiptDetail.receiptId}`);
  } catch (err) {
    console.error('Failed to add receipt item:', err);
    alert('Failed to add receipt item. Check the console for details.');
  }
}
</script>