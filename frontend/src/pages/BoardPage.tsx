import { useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useProject } from '../api/useProjects'
import { useTasks, useCreateTask } from '../api/useTasks'
import TaskColumn from '../components/TaskColumn'
import type { Priority } from '../types'

export default function BoardPage() {
  const { id = '' } = useParams()
  const { data: project } = useProject(id)
  const { data: tasks = [], isLoading } = useTasks(id)
  const createTask = useCreateTask(id)

  const [showForm, setShowForm] = useState(false)
  const [title, setTitle] = useState('')
  const [priority, setPriority] = useState<Priority>('Medium')

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault()
    await createTask.mutateAsync({ title, priority })
    setTitle('')
    setPriority('Medium')
    setShowForm(false)
  }

  const todo = tasks.filter((t) => t.status === 'Todo')
  const inProgress = tasks.filter((t) => t.status === 'InProgress')
  const done = tasks.filter((t) => t.status === 'Done')

  return (
    <div className="min-h-screen bg-white dark:bg-gray-900 p-8">
      <div className="max-w-6xl mx-auto">
        <header className="flex items-center gap-4 mb-6">
          <Link to="/projects" className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">← Projects</Link>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">{project?.name}</h1>
          <button
            onClick={() => setShowForm(!showForm)}
            className="ml-auto bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 text-sm"
          >
            Add task
          </button>
        </header>

        {showForm && (
          <form onSubmit={handleCreate} className="bg-gray-50 dark:bg-gray-800 rounded-xl p-4 mb-6 flex gap-3 items-end">
            <input
              type="text"
              placeholder="Task title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
              className="flex-1 border rounded-lg px-3 py-2 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
            />
            <select
              value={priority}
              onChange={(e) => setPriority(e.target.value as Priority)}
              className="border rounded-lg px-3 py-2 dark:bg-gray-700 dark:border-gray-600 dark:text-white"
            >
              <option>High</option>
              <option>Medium</option>
              <option>Low</option>
            </select>
            <button type="submit" className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700">
              Add
            </button>
            <button type="button" onClick={() => setShowForm(false)} className="text-gray-500 px-3 py-2">Cancel</button>
          </form>
        )}

        {isLoading ? (
          <p className="text-gray-500">Loading tasks…</p>
        ) : (
          <div className="flex gap-4 overflow-x-auto pb-4">
            <TaskColumn status="Todo" tasks={todo} />
            <TaskColumn status="InProgress" tasks={inProgress} />
            <TaskColumn status="Done" tasks={done} />
          </div>
        )}
      </div>
    </div>
  )
}
