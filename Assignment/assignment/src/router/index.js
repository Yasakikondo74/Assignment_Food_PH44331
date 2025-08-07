import { createRouter, createWebHistory } from 'vue-router';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/main.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/login.vue'),
    },
    {
      path: '/signup',
      name: 'signup',
      component: () => import('../views/signup.vue'),
    },
    {
      path: '/client',
      name: 'client',
      children: [
        {
          path: 'food',
          name: 'food-list-client',
          children: [
            {
              path: 'list',
              name: 'list-client',
              component: () => import('../views/client/list.vue'),
            },
            {
              path: 'detail/:id',
              name: 'food-detail-client',
              component: () => import('../views/client/detail.vue'),
            },
            {
              path: 'buy',
              name: 'buy-client',
              component: () => import('../views/client/buy.vue'),
            }
          ],
        },
      ],
    },
    {
      path: '/admin',
      name: 'admin',
      children: [
        {
          path: 'food',
          name: 'food-admin',
          children: [
            {
              path: 'list',
              name: 'food-list-admin',
              component: () => import('../views/admin/food/list.vue'),
            },
            {
              path: 'add',
              name: 'food-add-admin',
              component: () => import('../views/admin/food/add.vue'),
            },
            {
              path: 'detail/:id',
              name: 'food-detail-admin',
              component: () => import('../views/admin/food/detail.vue'),
            },
          ],
        },
        {
          path: 'account',
          name: 'account-admin',
          children: [
            {
              path: 'list',
              name: 'account-list-admin',
              component: () => import('../views/admin/account/list.vue'),
            },
            {
              path: 'add',
              name: 'account-add-admin',
              component: () => import('../views/admin/account/add.vue'),
            },
            {
              path: 'detail/:id',
              name: 'account-detail-admin',
              component: () => import('../views/admin/account/detail.vue'),
            },
          ],
        },
        {
          path: 'receipt_detail',
          name: 'receipt_detail-admin',
          children: [
            {
              path: 'list',
              name: 'receipt_detail-list-admin',
              component: () => import('../views/admin/receipt_detail/list.vue'),
            },
            {
              path: 'add',
              name: 'receipt_detail-add-admin',
              component: () => import('../views/admin/receipt_detail/add.vue'),
            },
            {
              path: 'detail/:id',
              name: 'receipt_detail-detail-admin',
              component: () => import('../views/admin/receipt_detail/detail.vue'),
            },
          ],
        },
        {
          path: 'receipt',
          name: 'receipt-admin',
          children: [
            {
              path: 'list',
              name: 'receipt-list-admin',
              component: () => import('../views/admin/receipt/list.vue'),
            },
            {
              path: 'add',
              name: 'receipt-add-admin',
              component: () => import('../views/admin/receipt/add.vue'),
            },
            {
              path: 'detail/:id',
              name: 'receipt-detail-admin',
              component: () => import('../views/admin/receipt/detail.vue'),
            },
          ],
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('../views/NotFound.vue'),
    },
  ],
});

router.afterEach(() => {
  // This is a good place to add things like a progress bar or analytics
  // after each navigation.
});

export default router;