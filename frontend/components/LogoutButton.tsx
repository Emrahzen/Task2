"use client"
import { useRouter } from 'next/navigation'

export default function LogoutButton({ locale = 'tr' }: { locale?: string }) {
  const router = useRouter()
  
  const handleLogout = () => {
    localStorage.removeItem('token')
    router.push(`/${locale}/auth/login`)
  }
  
  return (
    <button
      onClick={handleLogout}
      className="text-red-600 hover:text-red-700 font-medium text-sm"
    >
      {locale === 'tr' ? 'Çıkış Yap' : 'Logout'}
    </button>
  )
} 