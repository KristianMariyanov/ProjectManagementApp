import {
  createContext,
  useContext,
  useState,
  useCallback,
  type ReactNode,
} from 'react'
import { apiRequest, setTokens, clearTokens, hasTokens } from '../lib/apiClient'

interface AuthContextValue {
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(hasTokens)

  const login = useCallback(async (email: string, password: string) => {
    const resp = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    })
    if (!resp.ok) {
      const data = await resp.json().catch(() => null)
      throw new Error(data?.detail ?? data?.title ?? 'Login failed')
    }
    const data = await resp.json()
    setTokens(data.accessToken, data.refreshToken)
    setIsAuthenticated(true)
  }, [])

  const register = useCallback(
    async (email: string, password: string) => {
      const resp = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
      })
      if (!resp.ok) {
        const data = await resp.json().catch(() => null)
        const firstError = data?.errors ? Object.values(data.errors).flat()[0] : null
        throw new Error((firstError as string) ?? 'Registration failed')
      }
      await login(email, password)
    },
    [login]
  )

  const logout = useCallback(async () => {
    try {
      await apiRequest('/api/auth/logout', { method: 'POST' })
    } finally {
      clearTokens()
      setIsAuthenticated(false)
    }
  }, [])

  return (
    <AuthContext.Provider value={{ isAuthenticated, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}
