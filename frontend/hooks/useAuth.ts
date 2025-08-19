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
    
    setIsAuthenticated(true)
    setIsLoading(false)
  }, [router])

  const logout = () => {
    localStorage.removeItem('token')
    setIsAuthenticated(false)
    router.push('/tr/auth/login')
  }

  return { isAuthenticated, isLoading, logout }
} 