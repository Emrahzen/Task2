"use client"
import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'

export function useAuth() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [isLoading, setIsLoading] = useState(true)
  const router = useRouter()

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      router.push('/tr/auth/login')
      return
    }
    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      const expSeconds = payload.exp as number | undefined
      if (expSeconds && Date.now() >= expSeconds * 1000) {
        localStorage.removeItem('token')
        router.push('/tr/auth/login')
        return
      }
      setIsAuthenticated(true)
      setIsLoading(false)

      if (expSeconds) {
        const msUntilExpiry = expSeconds * 1000 - Date.now()
        const timer = setTimeout(() => {
          localStorage.removeItem('token')
          setIsAuthenticated(false)
          router.push('/tr/auth/login')
        }, Math.max(0, msUntilExpiry))
        return () => clearTimeout(timer)
      }
    } catch {
      // invalid token -> logout
      localStorage.removeItem('token')
      router.push('/tr/auth/login')
      return
    }
  }, [router])

  const logout = () => {
    localStorage.removeItem('token')
    setIsAuthenticated(false)
    router.push('/tr/auth/login')
  }

  return { isAuthenticated, isLoading, logout }
} 