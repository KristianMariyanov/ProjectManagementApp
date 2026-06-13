import { useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { useProjects, useCreateProject } from '../api/useProjects'

export default function ProjectsPage() {
  const { logout } = useAuth()
  const { data: projects, isLoading } = useProjects()
  const createProject = useCreateProject()
  const [showForm, setShowForm] = useState(false)
  const [name, setName] = useState('')
  const [description, setDescription] = useState('')

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault()
    await createProject.mutateAsync({ name, description })
    setName('')
    setDescription('')
    setShowForm(false)
  }

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900 p-8">
      <div className="max-w-4xl mx-auto">
        <header className="flex items-center justify-between mb-8">
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Projects</h1>
          <div className="flex gap-3">
            <button
              onClick={() => setShowForm(!showForm)}
              className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
            >
              New project
            </button>
            <button
              onClick={logout}
              className="text-gray-600 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white px-4 py-2"
            >
              Sign out
            </button>
          </div>
        </header>

        {showForm && (
          <form onSubmit={handleCreate} className="bg-white dark:bg-gray-800 rounded-xl p-6 mb-6 shadow space-y-3">
            <input
              type="text"
              placeholder="Project name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              className="w-full border rounded-lg px-3 py-2 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
            />
            <textarea
              placeholder="Description (optional)"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={2}
              className="w-full border rounded-lg px-3 py-2 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
            />
            <div className="flex gap-3">
              <button type="submit" className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700">
                Create
              </button>
              <button type="button" onClick={() => setShowForm(false)} className="text-gray-600 px-4 py-2">
                Cancel
              </button>
            </div>
          </form>
        )}

        {isLoading && <p className="text-gray-500">Loading…</p>}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {projects?.map((project) => (
            <Link
              key={project.id}
              to={`/projects/${project.id}`}
              className="bg-white dark:bg-gray-800 rounded-xl p-6 shadow hover:shadow-md transition-shadow"
            >
              <h2 className="text-lg font-semibold text-gray-900 dark:text-white">{project.name}</h2>
              {project.description && (
                <p className="text-gray-500 dark:text-gray-400 text-sm mt-1 line-clamp-2">{project.description}</p>
              )}
              <p className="text-xs text-gray-400 mt-2">{project.members.length} member{project.members.length !== 1 ? 's' : ''}</p>
            </Link>
          ))}
        </div>
      </div>
    </div>
  )
}
