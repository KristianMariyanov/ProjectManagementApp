import { test, expect } from '@playwright/test'

test('login flow', async ({ page }) => {
  await page.goto('/login')
  await page.fill('input[type="email"]', 'alice@example.com')
  await page.fill('input[type="password"]', 'Password1!')
  await page.click('button[type="submit"]')
  await expect(page).toHaveURL('/projects')
  await expect(page.getByText('Projects')).toBeVisible()
})
