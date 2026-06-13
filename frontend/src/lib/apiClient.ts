// API client — attaches access token, handles 401 by attempting one refresh
let accessToken: string | null = null
let refreshToken: string | null = null

export function setTokens(access: string, refresh: string) {
  accessToken = access
  refreshToken = refresh
}

export function clearTokens() {
  accessToken = null
  refreshToken = null
}

export function hasTokens() {
  return !!accessToken
}

async function refreshTokens(): Promise<boolean> {
  if (!refreshToken) return false
  try {
    const resp = await fetch('/api/auth/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken }),
    })
    if (!resp.ok) {
      clearTokens()
      return false
    }
    const data = await resp.json()
    if (data.accessToken) {
      setTokens(data.accessToken, data.refreshToken)
      return true
    }
    clearTokens()
    return false
  } catch {
    clearTokens()
    return false
  }
}

export async function apiRequest<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string>),
  }
  if (accessToken) headers['Authorization'] = `Bearer ${accessToken}`

  let resp = await fetch(path, { ...options, headers })

  if (resp.status === 401 && refreshToken) {
    const refreshed = await refreshTokens()
    if (refreshed && accessToken) {
      headers['Authorization'] = `Bearer ${accessToken}`
      resp = await fetch(path, { ...options, headers })
    }
  }

  if (!resp.ok) {
    const body = await resp.json().catch(() => ({ error: { message: resp.statusText } }))
    throw new Error(body.error?.message ?? `Request failed: ${resp.status}`)
  }

  return resp.json() as Promise<T>
}

export const api = {
  get: <T>(path: string) => apiRequest<T>(path),
  post: <T>(path: string, body: unknown) =>
    apiRequest<T>(path, { method: 'POST', body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    apiRequest<T>(path, { method: 'PUT', body: JSON.stringify(body) }),
  delete: <T>(path: string) => apiRequest<T>(path, { method: 'DELETE' }),
}
