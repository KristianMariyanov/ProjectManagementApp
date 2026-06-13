import type { TaskItem, TaskStatus } from '../types'
import TaskCard from './TaskCard'

interface TaskColumnProps {
  status: TaskStatus
  tasks: TaskItem[]
}

const columnLabels: Record<TaskStatus, string> = {
  Todo: 'To Do',
  InProgress: 'In Progress',
  Done: 'Done',
}

const columnColors: Record<TaskStatus, string> = {
  Todo: 'border-t-gray-400',
  InProgress: 'border-t-indigo-500',
  Done: 'border-t-green-500',
}

export default function TaskColumn({ status, tasks }: TaskColumnProps) {
  return (
    <div className={`flex-1 min-w-64 bg-gray-50 dark:bg-gray-900 rounded-xl p-4 border-t-4 ${columnColors[status]}`}>
      <h3 className="font-semibold text-gray-700 dark:text-gray-300 mb-3 flex items-center justify-between">
        {columnLabels[status]}
        <span className="text-xs bg-gray-200 dark:bg-gray-700 text-gray-600 dark:text-gray-400 px-2 py-0.5 rounded-full">
          {tasks.length}
        </span>
      </h3>
      <div className="space-y-2">
        {tasks.map((task) => (
          <TaskCard key={task.id} task={task} />
        ))}
      </div>
    </div>
  )
}
