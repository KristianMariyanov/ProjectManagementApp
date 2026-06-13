import { test, expect } from '@playwright/test'

test.beforeEach(async ({ page }) => {
  await page.goto('/login')
  await page.fill('input[type="email"]', 'alice@example.com')
  await page.fill('input[type="password"]', 'Password1!')
  await page.click('button[type="submit"]')
  await expect(page).toHaveURL('/projects')
})

test('create a task', async ({ page }) => {
  // Navigate to the first project board
  await page.getByRole('link').first().click()
  await expect(page.getByText('Add task')).toBeVisible()

  await page.getByText('Add task').click()
  await page.fill('input[placeholder="Task title"]', 'E2E test task')
  await page.click('button[type="submit"]')

  await expect(page.getByText('E2E test task')).toBeVisible()
})
