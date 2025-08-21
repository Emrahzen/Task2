"use client"
import { useEffect } from 'react'
import { useRouter } from 'next/navigation'

function getToken(): string | null {
  if (typeof window === 'undefined') return null
  try {
    return localStorage.getItem('token')
  } catch {
    return null
  }
}

function getExpiryMs(token: string | null): number | null {
  if (!token) return null
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    const expSeconds = payload?.exp as number | undefined
    if (!expSeconds) return null
    return expSeconds * 1000
  } catch {
    return null
  }
}

export default function AuthSessionWatcher({ locale = 'tr' }: { locale?: string }) {
  const router = useRouter()

  useEffect(() => {
    function enforce() {
      const token = getToken()
      const expMs = getExpiryMs(token)
      if (!token || (expMs !== null && Date.now() >= expMs)) {
        try { localStorage.removeItem('token') } catch {}
        router.push(`/${locale}/auth/login`)
        return { scheduled: false }
      }
      if (expMs !== null) {
        const delay = Math.max(0, expMs - Date.now())
        const t = setTimeout(() => {
          try { localStorage.removeItem('token') } catch {}
          router.push(`/${locale}/auth/login`)
        }, delay)
        return { scheduled: true, timer: t }
      }
      return { scheduled: false }
    }

    const result = enforce()
    const onVisibility = () => enforce()
    document.addEventListener('visibilitychange', onVisibility)

    return () => {
      document.removeEventListener('visibilitychange', onVisibility)
      if (result && result.scheduled && result.timer) clearTimeout(result.timer as unknown as number)
    }
  }, [router, locale])

  return null
}

